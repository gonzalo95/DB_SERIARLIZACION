using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace Toolbox
{
    public class Toolbox
    {
        // [ExpectedException(typeof(TrackingIdRepetidoException))] TEST UNITARIOS.
        public List<Equipo> Leer(Letras letra)
        {
            List<Equipo> salida = new List<Equipo>();
            Equipo equipo;
            SqlConnection conexion = new SqlConnection("Data Source=.;Initial Catalog=mundial-sp-2018;Integrated Security=True");
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            conexion.Open();
            comando.CommandText = string.Format("SELECT id,nombre FROM dbo.Equipos WHERE grupo='{0}'", letra);

            SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                equipo = new Equipo(Convert.ToInt32(lector["id"]), lector["nombre"].ToString());
                salida.Add(equipo);
            }
            conexion.Close();
            return salida;
        }

        // UPDATE: "UPDATE tabla SET campo = "gonzalo" WHERE dni =38587636"
        // DELETE: "DELETE FROM tabla WHERE id = 15"

        public bool Insertar(Paquete p)
        {
            try
            {
                SqlConnection conexion = new SqlConnection("Data Source=.;Initial Catalog=correo-sp-2017;Integrated Security=True");
                SqlCommand comando = new SqlCommand();
                comando.CommandType = System.Data.CommandType.Text;
                comando.Connection = conexion;
                comando.CommandText = string.Format("INSERT INTO dbo.Paquetes (direccionEntrega, trackingId, alumno) VALUES ('{0}','{1}','Greco Gonzalo')", p.DireccionEntrega, p.TrackingID);
                conexion.Open();
                comando.ExecuteNonQuery();
                conexion.Close();
                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public static bool GuardarTxt(this string texto, string archivo)
        {
            bool salida = false;
            if (archivo != null)
            {
                StreamWriter escritor = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + archivo, true);
                escritor.WriteLine(texto);
                salida = true;
                escritor.Close();
            }
            return salida;
        }


        public string Leertxt(string archivo)
        {
            string salida;
            StreamReader lector = new StreamReader(archivo);
            salida = lector.ReadToEnd();
            return salida;
        }

        public T LeerXml(string ruta)
        {
            try
            {
                XmlSerializer serializador = new XmlSerializer(typeof(T));
                XmlTextReader lector = new XmlTextReader(ruta);
                T dato = (T)serializador.Deserialize(lector);
                lector.Close();
                return dato;
            }
            catch (Exception exc)
            {
                throw new ErrorArchivoException("Error al leer", exc);
            }
        }

        // [XmlInclude(typeof(Clase))]

        public bool GuardarXml(string ruta, T objeto)
        {
            try
            {
                XmlSerializer serializador = new XmlSerializer(typeof(T));
                XmlTextWriter escritor = new XmlTextWriter(ruta, Encoding.UTF8);
                serializador.Serialize(escritor, objeto);
                escritor.Close();
            }
            catch (Exception exc)
            {
                throw new ErrorArchivoException("Error al guardar", exc);
            }
            return true;
        }

        private void GuardarBinario(object sender, EventArgs e)
        {
            FileStream archivo = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Pacientes.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(archivo, this.pacientesEnEspera);
            archivo.Close();
        }

        private void LeerBinario(object sender, EventArgs e)
        {
            FileStream archivo = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Pacientes.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            this.pacientesEnEspera = (Queue<Paciente>)bf.Deserialize(archivo);
            archivo.Close();
        }
    }
}
