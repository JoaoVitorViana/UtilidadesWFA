using System;
using System.Collections.Generic;
using System.Data;

namespace UtilidadesWFA.Db
{
    public class Parametro
    {
        private string Campo { get; set; }
        private object Valor { get; set; }
        private DbType TipoDados { get; set; }
        private Operador Operador { get; set; }

        public Parametro(string pCampo, object pValor, DbType pTipoDados, Operador pOperador)
        {
            Campo = pCampo;
            Valor = pValor;
            TipoDados = pTipoDados;
            Operador = pOperador;
        }

        public Parametro(string pCampo, object pValor, DbType pTipoDados)
        {
            Campo = pCampo;
            Valor = pValor;
            TipoDados = pTipoDados;
            Operador = Operador.Nenhum;
        }

        public Parametro(string pCampo, object pValor)
        {
            Campo = pCampo;
            Valor = pValor;
            Type t = pValor.GetType();
            TipoDados = (DbType)Enum.Parse(typeof(DbType), t.Name);
            Operador = Operador.Nenhum;
        }

        public Parametro(string pCampo, object pValor, Operador pOperador)
        {
            Campo = pCampo;
            Valor = pValor;
            Type t = pValor.GetType();
            TipoDados = (DbType)Enum.Parse(typeof(DbType), t.Name);
            Operador = pOperador;
        }

        private string CampoTabela(string pCampo, TpBanco pTipoBanco)
        {
            string CampoTabela = string.Empty;
            if (TipoDados == DbType.Date)
            {
                switch (pTipoBanco)
                {
                    case TpBanco.SqlServer:
                        CampoTabela = "CONVERT(DATE, [" + pCampo + "], 103)";
                        break;
                    case TpBanco.Sqlite:
                        CampoTabela = "DATE(" + pCampo + ")";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (TipoDados == DbType.DateTime)
            {
                switch (pTipoBanco)
                {
                    case TpBanco.SqlServer:
                        CampoTabela = "CONVERT(DATETIME, [" + pCampo + "], 120)";
                        break;
                    case TpBanco.Sqlite:
                        CampoTabela = "DATETIME(" + pCampo + ")";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                CampoTabela = "[" + pCampo + "]";
            }
            return CampoTabela;
        }

        private string CampoVarivel(string pCampo, string pParametro, TpBanco pTipoBanco)
        {
            string CampoVarivel = string.Empty;
            if (TipoDados == DbType.Date)
            {
                switch (pTipoBanco)
                {
                    case TpBanco.SqlServer:
                        CampoVarivel = "CONVERT(DATE, " + pParametro + pCampo + ", 103)";
                        break;
                    case TpBanco.Sqlite:
                        CampoVarivel = "DATE(" + pParametro + pCampo + ")";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (TipoDados == DbType.DateTime)
            {
                switch (pTipoBanco)
                {
                    case TpBanco.SqlServer:
                        CampoVarivel = "CONVERT(DATETIME, " + pParametro + pCampo + ", 120)";
                        break;
                    case TpBanco.Sqlite:
                        CampoVarivel = "DATETIME(" + pParametro + pCampo + ")";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                CampoVarivel = pParametro + pCampo;
            }
            return CampoVarivel;
        }

        private string Condicao(string pCampo, string pParametro, TpBanco pTipoBanco)
        {
            string strFormato = "";
            if (Operador == Operador.Igual || Operador == Operador.Nenhum)
                strFormato = "{0} = {1}";
            else if (Operador == Operador.Maior)
                strFormato = "{0} > {1}";
            else if (Operador == Operador.Menor)
                strFormato = "{0} < {1}";
            else if (Operador == Operador.MaiorIgual)
                strFormato = "{0} >= {1}";
            else if (Operador == Operador.MenorIgual)
                strFormato = "{0} <= {1}";
            else if (Operador == Operador.Diferente)
                strFormato = "{0} <> {1}";
            else if (Operador == Operador.Inicie)
                strFormato = "{0} LIKE {1} + '%'";
            else if (Operador == Operador.Termine)
                strFormato = "{0} LIKE '%' + {1}";
            else if (Operador == Operador.Contenha)
                strFormato = "{0} LIKE '%' + {1} + '%'";
            else if (Operador == Operador.Nulo)
                strFormato = "{0} IS NULL";
            else if (Operador == Operador.NaoNulo)
                strFormato = "{0} IS NOT NULL";
            else if (Operador == Operador.Vazio)
                strFormato = "COALESCE({0}, '') = ''";
            else if (Operador == Operador.DiferenteVazio)
                strFormato = "COALESCE({0}, '') <> ''";

            return string.Format(strFormato, CampoTabela(pCampo, pTipoBanco), CampoVarivel(pCampo, pParametro, pTipoBanco));
        }

        public string GetCondicao(string pParametro, TpBanco pTipoBanco)
        {
            string retorno = string.Empty;
            if (Valor.GetType().IsArray)
            {
                retorno = CampoTabela(Campo, pTipoBanco) + " IN (";
                for (int i = 0; i < ((Array)Valor).Length; i++)
                    retorno += CampoVarivel(Campo + i, pParametro, pTipoBanco) + (i < ((Array)Valor).Length - 1 ? "," : string.Empty);
                retorno += ")";
            }
            else
                retorno = Condicao(Campo, pParametro, pTipoBanco);
            return retorno;
        }

        public List<DBParametros> GetParametros()
        {
            List<DBParametros> pmts = new List<DBParametros>();
            if (Valor.GetType().IsArray)
            {
                for (int i = 0; i < ((Array)Valor).Length; i++)
                    pmts.Add(new DBParametros { Value = ((Array)Valor).GetValue(i), Name = Campo + i });
            }
            else
                pmts.Add(new DBParametros { Value = Valor, Name = Campo });
            return pmts;
        }
    }
}