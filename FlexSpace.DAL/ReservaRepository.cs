using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace FlexSpace.DAL
{
    public class ReservaRepository
    {
        private readonly string _connectionString = "Server=localhost;Database=flexspacedb;Uid=root;Pwd=;";

        public Cliente ObtenerClientePorId(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Nombre, Email, TipoCliente, SancionesActivas FROM Cliente WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Cliente
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Email = reader.GetString("Email"),
                                TipoCliente = reader.GetString("TipoCliente"),
                                SancionesActivas = reader.GetInt32("SancionesActivas")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Cliente> ObtenerClientesSancionados()
        {
            var lista = new List<Cliente>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Nombre, Email, TipoCliente, SancionesActivas FROM Cliente WHERE SancionesActivas > 0";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Cliente
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Email = reader.GetString("Email"),
                                TipoCliente = reader.GetString("TipoCliente"),
                                SancionesActivas = reader.GetInt32("SancionesActivas")
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void IncrementarSancionCliente(int clienteId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Cliente SET SancionesActivas = SancionesActivas + 1 WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", clienteId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Puesto ObtenerPuestoPorId(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Codigo, TipoPuesto, TarifaBasePorHora FROM Puesto WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Puesto
                            {
                                Id = reader.GetInt32("Id"),
                                Codigo = reader.GetString("Codigo"),
                                TipoPuesto = reader.GetString("TipoPuesto"),
                                TarifaBasePorHora = reader.GetDecimal("TarifaBasePorHora")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Reserva> ObtenerReservasConfirmadasPorPuesto(int puestoId)
        {
            var reservas = new List<Reserva>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal " +
                               "FROM Reserva WHERE PuestoId = @PuestoId AND Estado = 'Confirmada'";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PuestoId", puestoId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(new Reserva
                            {
                                Id = reader.GetInt32("Id"),
                                ClienteId = reader.GetInt32("ClienteId"),
                                PuestoId = reader.GetInt32("PuestoId"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Estado = reader.GetString("Estado"),
                                CostoTotal = reader.GetDecimal("CostoTotal")
                            });
                        }
                    }
                }
            }
            return reservas;
        }

        public Reserva ObtenerReservaPorId(int id)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal FROM Reserva WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Reserva
                            {
                                Id = reader.GetInt32("Id"),
                                ClienteId = reader.GetInt32("ClienteId"),
                                PuestoId = reader.GetInt32("PuestoId"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Estado = reader.GetString("Estado"),
                                CostoTotal = reader.GetDecimal("CostoTotal")
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void GuardarReserva(Reserva reserva)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Reserva (ClienteId, PuestoId, FechaInicio, FechaFin, Estado, CostoTotal) " +
                               "VALUES (@ClienteId, @PuestoId, @FechaInicio, @FechaFin, @Estado, @CostoTotal)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClienteId", reserva.ClienteId);
                    cmd.Parameters.AddWithValue("@PuestoId", reserva.PuestoId);
                    cmd.Parameters.AddWithValue("@FechaInicio", reserva.FechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", reserva.FechaFin);
                    cmd.Parameters.AddWithValue("@Estado", reserva.Estado);
                    cmd.Parameters.AddWithValue("@CostoTotal", reserva.CostoTotal);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void CancelarReserva(int reservaId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Reserva SET Estado = 'Cancelada' WHERE Id = @Id";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", reservaId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Reserva> ObtenerReservasFuturasPorCodigoPuesto(string codigoPuesto)
        {
            var reservas = new List<Reserva>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT r.Id, r.ClienteId, r.PuestoId, r.FechaInicio, r.FechaFin, r.Estado, r.CostoTotal " +
                               "FROM Reserva r INNER JOIN Puesto p ON r.PuestoId = p.Id " +
                               "WHERE p.Codigo = @Codigo AND r.Estado = 'Confirmada' AND r.FechaInicio > NOW()";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Codigo", codigoPuesto);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(new Reserva
                            {
                                Id = reader.GetInt32("Id"),
                                ClienteId = reader.GetInt32("ClienteId"),
                                PuestoId = reader.GetInt32("PuestoId"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                Estado = reader.GetString("Estado"),
                                CostoTotal = reader.GetDecimal("CostoTotal")
                            });
                        }
                    }
                }
            }
            return reservas;
        }
    }
}