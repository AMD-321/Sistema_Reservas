using System;
using System.Collections.Generic;
using FlexSpace.BLL;
using FlexSpace.DAL;

namespace FlexSpace.UI
{
    internal class Program
    {
        private static ReservaService _servicio = new ReservaService();

        static void Main(string[] args)
        {
            bool salir = false;

            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("");
                Console.WriteLine("Sistema de Gestion FlexSpace");
                Console.WriteLine("");
                Console.WriteLine("1. Registrar Nueva Reserva");
                Console.WriteLine("2. Cancelar Reserva");
                Console.WriteLine("3. Consultar Reservas Activas por Puesto");
                Console.WriteLine("4. Listar Clientes Sancionados");
                Console.WriteLine("5. Salir");
                Console.WriteLine("");
                Console.Write("Seleccione una opción: ");

                string opcion = Console.ReadLine() ?? string.Empty;

                switch (opcion)
                {
                    case "1":
                        RegistrarReserva();
                        break;
                    case "2":
                        CancelarReserva();
                        break;
                    case "3":
                        ConsultarPorPuesto();
                        break;
                    case "4":
                        ListarSancionados();
                        break;
                    case "5":
                        salir = true;
                        break;
                    default:
                        Pausar("Opción no válida.");
                        break;
                }
            }
        }

        private static void RegistrarReserva()
        {
            Console.Clear();
            Console.WriteLine("1. Registrar nueva reserva");

            try
            {
                Console.Write("Ingrese Cliente ID: ");
                if (!int.TryParse(Console.ReadLine(), out int clienteId))
                {
                    Pausar("El ID de cliente debe ser un entero.");
                    return;
                }

                Cliente cliente = _servicio.ObtenerCliente(clienteId);
                if (cliente == null)
                {
                    Pausar("No se encontró el cliente en la Base de Datos.");
                    return;
                }

                Console.Write("Ingrese Puesto ID: ");
                if (!int.TryParse(Console.ReadLine(), out int puestoId))
                {
                    Pausar("El ID de puesto debe ser un entero.");
                    return;
                }

                Puesto puesto = _servicio.ObtenerPuesto(puestoId);
                if (puesto == null)
                {
                    Pausar("No se encontró el puesto en la Base de Datos.");
                    return;
                }

                Console.Write("Fecha Inicio (yyyy-MM-dd HH:mm): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime inicio))
                {
                    Pausar("Formato de fecha de inicio inválido.");
                    return;
                }

                Console.Write("Fecha Fin (yyyy-MM-dd HH:mm): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime fin))
                {
                    Pausar("Formato de fecha de fin inválido.");
                    return;
                }

                decimal tarifa = _servicio.CalcularTarifa(cliente, puesto, inicio, fin);

                Console.WriteLine("");
                Console.WriteLine("Resumen Previo de la Reserva");
                Console.WriteLine($"Cliente: {cliente.Nombre} ({cliente.TipoCliente})");
                Console.WriteLine($"Puesto: {puesto.Codigo} - {puesto.TipoPuesto}");
                Console.WriteLine($"Inicio: {inicio:g} | Fin: {fin:g}");
                Console.WriteLine($"Costo Total Calculado: ${tarifa:N2}");
                Console.WriteLine("");
                Console.Write("¿Desea confirmar y guardar la reserva? (si/no): ");

                if ((Console.ReadLine() ?? "").Equals("S", StringComparison.OrdinalIgnoreCase))
                {
                    Reserva r = _servicio.RegistrarReserva(cliente, puesto, inicio, fin);
                    Pausar($"Reserva guardada exitosamente ID Generado: {r.Id}");
                }
                else
                {
                    Pausar("Operación cancelada por el usuario.");
                }
            }
            catch (ClienteSancionadoException ex)
            {
                Pausar($"[Cliente bloqueado] {ex.Message}");
            }
            catch (Exception ex)
            {
                Pausar($"[Error de registro] {ex.Message}");
            }
        }

        private static void CancelarReserva()
        {
            Console.Clear();
            Console.WriteLine("2. Cancelar Reserva");
            Console.Write("Ingrese ID de la reserva a cancelar: ");

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                _servicio.CancelarReserva(id, out string msj);
                Pausar($"{msj}");
            }
            else
            {
                Pausar("ID de reserva no válido.");
            }
        }

        private static void ConsultarPorPuesto()
        {
            Console.Clear();
            Console.WriteLine("3. Consultar reservas activas por puesto");
            Console.Write("Ingrese código del puesto (ej: ESC-01): ");
            string cod = Console.ReadLine() ?? string.Empty;

            List<Reserva> lista = _servicio.ConsultarReservasActivasPorPuesto(cod);
            Console.WriteLine($"Reservas futuras encontradas: {lista.Count}");
            Console.WriteLine("");
            foreach (var r in lista)
            {
                Console.WriteLine($"Reserva ID: {r.Id} | Cliente ID: {r.ClienteId} | Desde: {r.FechaInicio:g} | Hasta: {r.FechaFin:g} | Total: ${r.CostoTotal:N2}");
            }
            Pausar(string.Empty);
        }

        private static void ListarSancionados()
        {
            Console.Clear();
            Console.WriteLine("4. Lista de clientes sancionados");

            List<Cliente> sancionados = _servicio.ObtenerClientesSancionados();
            if (sancionados.Count == 0)
            {
                Console.WriteLine("No hay clientes con sanciones activas.");
            }
            else
            {
                Console.WriteLine("ID | Nombre | Email | Tipo | Sanciones");
                Console.WriteLine("");
                foreach (var c in sancionados)
                {
                    Console.WriteLine($"{c.Id} | {c.Nombre} | {c.Email} | {c.TipoCliente} | {c.SancionesActivas}");
                }
            }
            Pausar(string.Empty);
        }

        private static void Pausar(string mensaje)
        {
            if (!string.IsNullOrEmpty(mensaje)) Console.WriteLine(mensaje);
            Console.ReadKey();
        }
    }
}