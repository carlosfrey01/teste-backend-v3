using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TheatricalPlayersRefactoringKata
{
    delegate int CalculatePerGender(int currentAmount, int audience);
    public class CalculatedPerformance
    {
        private int amount;
        private int seats;
        private int credits;
        private string playName;

        public int Amount { get => amount; set => amount = value; }
        public int Seats { get => seats; set => seats = value; }
        public int Credits { get => credits; set => credits = value; }
        public string PlayName { get => playName; set => playName = value; }

        public CalculatedPerformance(int amount, string playName, int seats, int credits)
        {
            this.amount = amount;
            this.playName = playName;
            this.seats = seats;
            this.credits = credits;
        }

    }
    public class InvoiceCalculation
    {

        private Invoice _invoice;
        private Dictionary<string, Play> _playMapper { get; set; }
        private Dictionary<string, CalculatePerGender> _genderCalculationMapper;
        private List<CalculatedPerformance> _calculatedPerformances;
        public List<CalculatedPerformance> CalculatedPerformances { get => _calculatedPerformances; set => _calculatedPerformances = value; }
        public int TotalAmount { get => _totalAmount; set => _totalAmount = value; }
        public int TotalCredits { get => _totalCredits; set => _totalCredits = value; }

        private int _totalCredits;
        private int _totalAmount;
        public InvoiceCalculation(Invoice invoice, Dictionary<string, Play> playMapper)
        {
            this._invoice = invoice;
            this._playMapper = playMapper;
            this._genderCalculationMapper = new Dictionary<string, CalculatePerGender> { { "tragedy", CalculateForTragedyGender }, { "comedy", CalculateForComedyGender }, { "history", CalculateForHistoryGender } };
            this._calculatedPerformances = new List<CalculatedPerformance>();
        }
        public int DefineBaseAmount(int currentNumberOfLines)
        {
            if (currentNumberOfLines < 1000) currentNumberOfLines = 1000;
            if (currentNumberOfLines > 4000) currentNumberOfLines = 4000;
            return currentNumberOfLines * 10;
        }

        public static int CalculateForTragedyGender(int currentAmount, int audience)
        {
            if (audience > 30)
            {
                currentAmount += 1000 * (audience - 30);
            }
            return currentAmount;
        }
        public static int CalculateForComedyGender(int currentAmount, int audience)
        {
            if (audience > 20)
            {
                currentAmount += 10000 + 500 * (audience - 20);
            }
            currentAmount += 300 * audience;
            return currentAmount;
        }

        public static int CalculateForHistoryGender(int currentAmount, int audience)
        {
            if(audience > 30) {
                currentAmount += (currentAmount * 35) + 480;
            } else {
                currentAmount += (currentAmount * audience) + 6000;
            }
            return currentAmount;
        }

        public CalculatedPerformance CalculatePerformance(Performance performance)
        {
            Play currentPlay = _playMapper.GetValueOrDefault(performance.PlayId, null);
            int amount = DefineBaseAmount(currentPlay.Lines);

            CalculatePerGender calculate = _genderCalculationMapper[currentPlay.Type];
            amount = calculate(amount, performance.Audience);

            int volumeCredits = Math.Max(performance.Audience - 30, 0);
            if ("comedy" == currentPlay.Type) volumeCredits += (int)Math.Floor((decimal)performance.Audience / 5);
            return new CalculatedPerformance(amount, currentPlay.Name, performance.Audience, volumeCredits);
        }
        public void CalculateInvoice()
        {
            if(_calculatedPerformances.Count > 0) _calculatedPerformances.Clear();
            if (_totalAmount > 0) _totalAmount = 0;
            if (_totalCredits > 0) _totalCredits = 0;

            foreach (Performance performance in _invoice.Performances) 
            {
                CalculatedPerformance totalPerformance = CalculatePerformance(performance);
                _calculatedPerformances.Add(totalPerformance);
                _totalCredits += totalPerformance.Credits;
                _totalAmount += totalPerformance.Amount;

            }
        }
    }


}

