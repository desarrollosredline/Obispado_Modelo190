using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "190 model files (*.190)|*.190|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    lbFiles.Items.Add(System.IO.Path.GetFullPath(filename));
                btnJoinFile.IsEnabled = true;
            }
            if (lbFiles.Items.Count != 2)
            {
                MessageBox.Show("Sólo se permiten seleccionar dos ficheros 190");
                lbFiles.Items.Clear();
                btnJoinFile.IsEnabled = false;
            }
        }

        private void btnRemoveFiles_Click(object sender, RoutedEventArgs e)
        {
            lbFiles.Items.Clear();
            btnJoinFile.IsEnabled = false;
        }

        private void btnJoinFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Recorrer po lineas
                var inputFiles = new[] { lbFiles.Items.GetItemAt(0).ToString(), lbFiles.Items.GetItemAt(1).ToString() };
                List<string> resultado = new List<string>();
                List<string> lineas = new List<string>();
                List<string> cabeceras = new List<string>();
                foreach (var file in inputFiles)
                {
                    var lines = File.ReadAllLines(file, Encoding.Default);
                    //Si quieres agregar solo un fichero a la cabecera
                    //if(file == lbFiles.Items.GetItemAt(0).ToString())
                    if (lines.ElementAt(0).Substring(0, 1) == "1")
                        cabeceras.Add(lines.ElementAt(0));
                    foreach (var s in lines.Skip(1).ToArray())
                    {
                        //if (!lineas.Contains(s) && s.Substring(0,1) == "2")
                        if (s.Substring(0, 1) == "2")
                        {
                            string milinea = s.Substring(0, 152) + "".PadRight(14, ' ') + "".PadRight(3, '0') + s.Substring(169, s.Length - 169);
                            lineas.Add(milinea);
                            if (file == lbFiles.Items.GetItemAt(0).ToString())
                            {
                                generarLista1(s, 1);
                            }
                            else
                            {
                                generarLista1(s, 2);
                            }
                        }
                    }
                }
                //Rellenar cabecera
                int NUMEROTOTALPERCEPCIONES = 0;
                int IMPORTETOTALPERCEPCIONES = 0;
                int TOTALRETENCIONESEINGRESOS = 0;
                foreach (var s in cabeceras)
                {
                    NUMEROTOTALPERCEPCIONES += Int32.Parse(s.Substring(136, 8));
                    IMPORTETOTALPERCEPCIONES += Int32.Parse(s.Substring(145, 15));
                    TOTALRETENCIONESEINGRESOS += Int32.Parse(s.Substring(160, 15));
                    //NUMEROTOTALPERCEPCIONES += 1;
                    //IMPORTETOTALPERCEPCIONES += Int32.Parse(s.Substring(81, 26));
                    //IMPORTETOTALPERCEPCIONES += Int32.Parse(s.Substring(108, 39));
                    //IMPORTETOTALPERCEPCIONES += Int32.Parse(s.Substring(197, 12));
                    //TOTALRETENCIONESEINGRESOS += Int32.Parse(s.Substring(138, 3));
                    //TOTALRETENCIONESEINGRESOS += Int32.Parse(s.Substring(184, 12));
                }
                string cabecera = cabeceras[0].ToString().Substring(0, 136);
                cabecera += NUMEROTOTALPERCEPCIONES.ToString().PadLeft(8, '0');
                cabecera += ' ';
                cabecera += IMPORTETOTALPERCEPCIONES.ToString().PadLeft(15, '0');
                cabecera += TOTALRETENCIONESEINGRESOS.ToString().PadLeft(15, '0');

                if (hayDuplicados())
                {
                    MessageBox.Show("DNI duplicado/s: " + Environment.NewLine + dni_duplicados);
                    dni_duplicados = "";
                    return;
                }

                //Crear string result
                resultado.Add(cabecera);
                foreach (var s in lineas)
                    resultado.Add(s);
                String path = "C:\\Users\\" + Environment.UserName + "\\Desktop\\Modelo190.190";
                File.WriteAllLines(path, resultado.ToArray(), Encoding.Default);
                MessageBox.Show("Realizado con exito");
                lbFiles.Items.Clear();
                btnOpenFile.IsEnabled = true;                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        List<string> file1DNIs = new List<string>();
        List<string> file2DNIs = new List<string>();
        string dni_duplicados = "";

        private void generarLista1(String s, int i)
        {
            String dni = s.Substring(17, 9);
            if (i == 1)
            {
                if (!file1DNIs.Contains(dni))
                {
                    file1DNIs.Add(dni);
                }
            }
            else if (i == 2)
            {
                if (!file2DNIs.Contains(dni))
                {
                    file2DNIs.Add(dni);
                }
            }
        }

        private Boolean hayDuplicados() 
        {
            Boolean resultado = false;
            foreach (String s in file1DNIs)
            {
                if (file2DNIs.Contains(s))
                {
                    dni_duplicados += s + Environment.NewLine;
                    resultado =  true;
                }      
            }
            return resultado;
        }

    }
}
