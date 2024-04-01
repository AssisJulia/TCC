using Newtonsoft.Json;

namespace TCC_SAMMI.Presentation
{
    /*public class Usuario
    {
        public string nome  { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public string datahora { get; set; }
        public IDictionary<string, int> matematica { get; set; }
    }*/

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class UserMatematica
    {
        public int adi { get; set; }
        public int sub { get; set; }
        public int mul { get; set; }
        public int div { get; set; }
    }

    public class UserPortugues
    {
        public int silabas { get; set; }
        public int animais { get; set; }
        public int paises { get; set; }
        public int frutas { get; set; }
    }

    public class Usuario
    {
        public string nome { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public string datahora { get; set; }
        public UserMatematica matematica { get; set; }
        public UserPortugues portugues { get; set; }
    }
}
