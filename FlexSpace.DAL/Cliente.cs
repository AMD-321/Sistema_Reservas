namespace FlexSpace.DAL
{
        public class Cliente
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string TipoCliente { get; set; } = string.Empty;
            public int SancionesActivas { get; set; }
        }
    }