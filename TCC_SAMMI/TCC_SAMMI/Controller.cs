using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Linq;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using static TCC_SAMMI.Presentation.Controller;

namespace TCC_SAMMI.Presentation
{
    public class Controller
    {
        //json to c# class https://json2csharp.com/
        public string pathuser = Path.Combine(Environment.CurrentDirectory, @"JSON\", "usuario.json");
        public string pathusers= Path.Combine(Environment.CurrentDirectory, @"JSON\", "usuarios.json");

        public void upadateusermatematica(UserMatematica newmatematica)
        {
            if (islogged())
            {
                Usuario usuario = getuser();
                usuario.matematica = newmatematica;

                string json = JsonConvert.SerializeObject(usuario, Formatting.Indented);
                System.IO.File.WriteAllText(pathuser, json);
            }
        }

        public void upadateuserportugues(UserPortugues newportu)
        {
            if (islogged())
            {
                Usuario usuario = getuser();
                usuario.portugues = newportu;

                string json = JsonConvert.SerializeObject(usuario, Formatting.Indented);
                System.IO.File.WriteAllText(pathuser, json);
            }
        }

        public bool islogged()
        {
            Usuario user = JsonConvert.DeserializeObject<Usuario>(File.ReadAllText(pathuser));
            return !string.IsNullOrEmpty(user.nome) ? true : false;
        }

        public dynamic getuser()
        {
           return JsonConvert.DeserializeObject<Usuario>(File.ReadAllText(pathuser));
        }

        public dynamic getusers()
        {
            string f = File.ReadAllText(pathusers);
            if (!string.IsNullOrEmpty(f)) { 
                return JsonConvert.DeserializeObject<List<Usuario>>(f);
            }
            else
            {
                List<Usuario> list = new List<Usuario> { };
                return list;
            }
        }

        public void logar(Usuario user)
        {
            string json = JsonConvert.SerializeObject(user, Formatting.Indented);
            System.IO.File.WriteAllText(pathuser, json);
        }

        public bool cadastrauser(IList<Usuario> list)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            System.IO.File.WriteAllText(pathusers, json);
            deslogar();
            logar(list.Last());

            return true;
        }

        public void deslogar()
        {
            //before logout, save the current user data in the users json file
            Usuario currentuser = getuser();
            List<Usuario> list = getusers();
            List<Usuario> newlist = new List<Usuario>();

            foreach (Usuario user in list)
            {
                if (user.nome == currentuser.nome && user.email == currentuser.email)
                {
                    newlist.Add(currentuser);
                }
                else
                {
                    newlist.Add(user);
                }
            }

            string js = JsonConvert.SerializeObject(newlist, Formatting.Indented);
            System.IO.File.WriteAllText(pathusers, js);

            //clean the user json file
            Usuario u = new Usuario();
            string json = JsonConvert.SerializeObject(u, Formatting.Indented);
            System.IO.File.WriteAllText(pathuser, json);
        }
    }
}
