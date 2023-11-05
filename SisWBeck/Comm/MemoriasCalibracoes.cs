using mkdinfo.communication.protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisWBeck.Comm
{
    public class MemoriasCalibracoes
    {
        public int Id { get; set; } = -1;
        public string Nome { get; set; } = "";
        public int NrPontos { get; set; } = 0;
        public int PesoPadrao { get; set; } = 0;
        public int FundoEscala { get; set; } = 0;

        public MemoriasCalibracoes() { }

        public MemoriasCalibracoes(ProtocoloModuloPesagemSMAX.SensorCalibrationResponse calibracao)
        {
            if (calibracao != null)
            {
                this.Id = calibracao.id;
                this.Nome = calibracao.name;
                this.NrPontos = calibracao.nrPontos;
                this.PesoPadrao = calibracao.pesoPadrao;
                this.FundoEscala = calibracao.fundoEscala;
            }
        }
    }
}
