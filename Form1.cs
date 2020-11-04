using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace ISI_IPMAForm
{
    public partial class Form1 : Form
    {
        // Variáveis globais
        static Dictionary<int, string> Locais { get; set; }

        /// <summary>
        /// Leitura do ficheiro com a informação acerca dos locais: locais.csv
        /// </summary>
        /// <param name="ficheiro">caminho ficheiro .csv</param>
        static Dictionary<int, string> LerLocais(string ficheiro)
        {
            Dictionary<int, string> dicLocais = new Dictionary<int, string>();

            // Expressão Regular para instanciar objeto Regex
            String erString = @"^[0-9]{7},[123],([1-9]?\d,){2}[A-Z]{3},([^,\n]*)$";
            Regex g = new Regex(erString);

            using (StreamReader r = new StreamReader(ficheiro))
            {
                string line;

                while ((line = r.ReadLine()) != null)
                {
                    // Tenta correspondência (macthing) da ER com cada linha do ficheiro
                    Match m = g.Match(line);

                    // Se não corresponder salta para a próxima iteração
                    if (!m.Success) continue;
                    
                    string[] campos = m.Value.Split(',');
                    int codLocal = int.Parse(campos[0]);
                    string cidade = campos[5];
                    dicLocais.Add(codLocal, cidade);
                }
            }

            return dicLocais;
        }

        static PrevisaoIPMA LerFicheiroPrevisao(int globalIdLocal)
        {
            String jsonString = null;
            using (StreamReader reader = new StreamReader(@"data_forecast/"+ globalIdLocal  + ".json"))
                jsonString = reader.ReadToEnd();

            PrevisaoIPMA obj = JsonConvert.DeserializeObject<PrevisaoIPMA>(jsonString);

            // Guardar local da previsão
            if (Locais.ContainsKey(obj.globalIdLocal))
                obj.local = Locais[obj.globalIdLocal];
            else
                obj.local = "Não detetado";

            return obj;
        }

        static void GuardarOutputs(List<PrevisaoIPMA> listaPrevisoes)
        {
            string json;
            foreach (PrevisaoIPMA previsao in listaPrevisoes)
            {
                json = JsonConvert.SerializeObject(previsao);

                // Verificar se o diretory output existe, se não cria.
                if (!Directory.Exists("output/")) Directory.CreateDirectory("output/");

                File.WriteAllText("output/" + previsao.globalIdLocal + ".json", json);
                
                XmlDocument xmlNode = JsonConvert.DeserializeXmlNode(json, "root");
                xmlNode.Save("output/" + previsao.globalIdLocal + ".xml");
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Locais = LerLocais("data_forecast/locais.csv");
            List<PrevisaoIPMA> listaPrevisoes = new List<PrevisaoIPMA>();
            int idPrevisao;

            foreach (string previsao in Directory.GetFiles("data_forecast/"))
            {
                if (!previsao.EndsWith(".json") || 
                    !int.TryParse(Path.GetFileNameWithoutExtension(previsao), out idPrevisao)) continue;

                listaPrevisoes.Add(LerFicheiroPrevisao(idPrevisao));
            }

            // Guardar outputs
            GuardarOutputs(listaPrevisoes);
        }
    }
}
