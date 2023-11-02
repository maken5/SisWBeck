using Modelo.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Converter
{
    internal static class ConvertersHelperExtensionMethods
    {
        public static string ToValidFileName(this string text)
        {
            if (text == null) return null;
            string fileName = text;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            if (String.IsNullOrWhiteSpace(fileName)) return "arquivo";   
            return fileName;
        }

        public static string QuoteStringToCSV(this string text, bool usarPontoVirgula=true)
        {
            string resposta = text;
            bool cotar = false;
            if (String.IsNullOrWhiteSpace(resposta)) return "";
            if (resposta.Contains("\""))
            {
                resposta = resposta.Replace("\"", "\"\"");
                cotar = true;
            }
            if (usarPontoVirgula)
            {
                cotar |= resposta.Contains(";");
                resposta = resposta.Replace(";", "\";\"");
            }
            else
            {
                cotar |= resposta.Contains(",");
                resposta = resposta.Replace(",", "\",\"");
            }
            if (cotar)
                resposta = $"\"{resposta}\"";
            return resposta;
        }

        public static string UltimaPesagemToCSVString(this List<Pesagens> pesagens, bool usarPontoVirgula = true)
        {
            string separador = usarPontoVirgula ? ";" : ",";
            string resposta = $"Identificação{separador}Peso{separador}Data Pesagem";
            if (pesagens != null && pesagens.Any())
            {
                foreach (var pe in pesagens)
                {
                    resposta += $"\r\n{pe.Codigo.QuoteStringToCSV(usarPontoVirgula)}{separador}{pe.Peso.ToString()}{separador}{pe.Data.ToString("yyyy-MM-dd")}";
                }
            }
            return resposta;
        }


        public static string Ultimas5PesagemToCSVString(this Dictionary<string,List<Pesagens>> pesagens, int primeiraPesagem=1, bool usarPontoVirgula = true)
        {
            string separador = usarPontoVirgula ? ";" : ",";
            if (primeiraPesagem < 1) primeiraPesagem = 1;
            string resposta = $"Identificação{separador}Peso {primeiraPesagem}{separador}Data {primeiraPesagem}{separador}Peso {primeiraPesagem+1}{separador}Data {primeiraPesagem+1}{separador}Peso {primeiraPesagem + 2}{separador}Data {primeiraPesagem + 2}{separador}Peso {primeiraPesagem + 3}{separador}Data {primeiraPesagem + 3}{separador}Peso {primeiraPesagem + 4}{separador}Data {primeiraPesagem + 4}";
            List<string> animais = pesagens.Keys.OrderBy(x => x).ToList();
            if (pesagens != null && pesagens.Any())
            {
                foreach (var cod in animais)
                {
                    List<Pesagens> pe = pesagens[cod];
                    if (pe != null && pe.Any())
                    {
                        resposta += $"\r\n{cod.QuoteStringToCSV(usarPontoVirgula)}{separador}";
                        Pesagens p = pe.Where(pp => pp.NrPesagem == primeiraPesagem).FirstOrDefault();
                        if (p != null)
                            resposta += $"{p.Peso.ToString()}{separador}{p.Data.ToString("yyyy-MM-dd HH:mm:ss")}{separador}";
                        else
                            resposta += $"{separador}{separador}"; 
                        p = pe.Where(pp => pp.NrPesagem == primeiraPesagem+1).FirstOrDefault();
                        if (p != null)
                            resposta += $"{p.Peso.ToString()}{separador}{p.Data.ToString("yyyy-MM-dd HH:mm:ss")}{separador}";
                        else
                            resposta += $"{separador}{separador}";
                        p = pe.Where(pp => pp.NrPesagem == primeiraPesagem + 2).FirstOrDefault();
                        if (p != null)
                            resposta += $"{p.Peso.ToString()}{separador}{p.Data.ToString("yyyy-MM-dd HH:mm:ss")}{separador}";
                        else
                            resposta += $"{separador}{separador}";
                        p = pe.Where(pp => pp.NrPesagem == primeiraPesagem + 3).FirstOrDefault();
                        if (p != null)
                            resposta += $"{p.Peso.ToString()}{separador}{p.Data.ToString("yyyy-MM-dd HH:mm:ss")}{separador}";
                        else
                            resposta += $"{separador}{separador}";
                        p = pe.Where(pp => pp.NrPesagem == primeiraPesagem + 4).FirstOrDefault();
                        if (p != null)
                            resposta += $"{p.Peso.ToString()}{separador}{p.Data.ToString("yyyy-MM-dd HH:mm:ss")}";
                        else
                            resposta += $"{separador}";
                    }
                }
            }
            return resposta;
        }
    }
}
