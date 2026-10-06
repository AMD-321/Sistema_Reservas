using System;
using System.Collections.Generic;
using FlexSpace.DAL;


namespace FlexSpace.BLL
{
    public class ReservaService
    {
        private readonly ReservaRepository _reservaRepo = new ReservaRepository();

        public Cliente ObtenerCliente(int id) => _reservaRepo.ObtenerClientePorId(id);
        public Puesto ObtenerPuesto(int id) => _reservaRepo.ObtenerPuestoPorId(id);

        public bool ValidarDisponibilidad(int puestoId, DateTime inicio, DateTime fin)
        {
            if (inicio >= fin) return false;

            List<Reserva> reservasConfirmadas = _reservaRepo.ObtenerReservasConfirmadasPorPuesto(puestoId);
            foreach (var r in reservasConfirmadas)
            {
                if (inicio < r.FechaFin && fin > r.FechaInicio)
                {
                    return false;
                }
            }
            return true;
        }

        public decimal CalcularTarifa(Cliente cliente, Puesto puesto, DateTime inicio, DateTime fin)
        {
            double totalHoras = (fin - inicio).TotalHours;

            decimal subtotalBase = puesto.TarifaBasePorHora * (decimal)totalHoras;

            if (cliente.SancionesActivas > 0)
            {
                return subtotalBase * 1.20m;
            }

            decimal acumulado = subtotalBase;

            if (IncluyeFinDeSemana(inicio, fin))
            {
                acumulado += subtotalBase * 0.15m;
            }

            if (totalHoras >= 5.0)
            {
                acumulado -= acumulado * 0.10m;
            }

            if (cliente.TipoCliente.Equals("VIP", StringComparison.OrdinalIgnoreCase))
            {
                acumulado -= acumulado * 0.05m;
            }

            return acumulado;
        }

        public Reserva RegistrarReserva(Cliente cliente, Puesto puesto, DateTime inicio, DateTime fin)
        {
            if (cliente.SancionesActivas >= 3)
            {
                throw new ClienteSancionadoException($"El cliente '{cliente.Nombre}' posee {cliente.SancionesActivas} sanciones activas y tiene el acceso bloqueado.");
            }

            if (inicio >= fin)
            {
                throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");
            }

            if (!ValidarDisponibilidad(puesto.Id, inicio, fin))
            {
                throw new InvalidOperationException("El puesto no se encuentra disponible en el horario solicitado.");
            }

            decimal costoTotal = CalcularTarifa(cliente, puesto, inicio, fin);

            var nuevaReserva = new Reserva
            {
                ClienteId = cliente.Id,
                PuestoId = puesto.Id,
                FechaInicio = inicio,
                FechaFin = fin,
                Estado = "Confirmada",
                CostoTotal = costoTotal
            };

            _reservaRepo.GuardarReserva(nuevaReserva);
            return nuevaReserva;
        }

        public bool CancelarReserva(int reservaId, out string mensaje)
        {
            Reserva reserva = _reservaRepo.ObtenerReservaPorId(reservaId);
            if (reserva == null)
            {
                mensaje = "No se encontró la reserva con el ID ingresado.";
                return false;
            }

            if (reserva.Estado == "Cancelada")
            {
                mensaje = "La reserva ya se encuentra cancelada.";
                return false;
            }

            _reservaRepo.CancelarReserva(reservaId);

            TimeSpan diferencia = reserva.FechaInicio - DateTime.Now;
            if (diferencia.TotalHours < 2)
            {
                _reservaRepo.IncrementarSancionCliente(reserva.ClienteId);
                mensaje = "Reserva cancelada. ATENCIÓN: Se aplicó +1 sanción al cliente por cancelar con menos de 2 horas de anticipación.";
            }
            else
            {
                mensaje = "Reserva cancelada exitosamente sin penalización.";
            }

            return true;
        }

        public List<Reserva> ConsultarReservasActivasPorPuesto(string codigoPuesto)
        {
            return _reservaRepo.ObtenerReservasFuturasPorCodigoPuesto(codigoPuesto);
        }

        public List<Cliente> ObtenerClientesSancionados()
        {
            return _reservaRepo.ObtenerClientesSancionados();
        }

        private bool IncluyeFinDeSemana(DateTime inicio, DateTime fin)
        {
            for (DateTime fecha = inicio.Date; fecha <= fin.Date; fecha = fecha.AddDays(1))
            {
                if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                {
                    return true;
                }
            }
            return false;
        }
    }
}