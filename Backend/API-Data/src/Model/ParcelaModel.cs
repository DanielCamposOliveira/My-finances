using API_Data.src.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Data.src.Model
{
    public class Parcela
    {
    
        public int Id { get; set; }

   
        public int NumeroParcela { get; set; }

  
        public decimal ValorParcela { get; set; }

     
        public DateTime DataVencimento { get; set; }


        public DateTime? DataPagamento { get; set; }


        public StatusParcela Status { get; set; } = StatusParcela.Aberto;

        public int LancamentoId { get; set; }
        public Lancamento Lancamento { get; set; } = null!;
    }
}
