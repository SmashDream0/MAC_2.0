using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Employee.Mechanisms;

namespace MAC_2.Employee.Windows.Norm
{
    public class NormPriceValue : NormBaseValue
    {
        public NormPriceValue(uint id)
        {
            _priceNorm = Helpers.LogicHelper.PriceNormLogic.FirstModel(id);

            this._pollutionID = _priceNorm.PollutionID;
            this._resolutionClarifyID = _priceNorm.ResolutionClarifyID;
        }

        public NormPriceValue(uint pollutionID, uint resolutionClarifyID)
        {
            this._pollutionID = pollutionID;
            this._resolutionClarifyID = resolutionClarifyID;
        }

        private readonly uint _pollutionID;
        private readonly uint _resolutionClarifyID;

        private Model.PriceNorm _priceNorm;

        public override string Value
        {
            get
            {
                if (Price == 0)
                { return String.Empty; }
                else
                { return gecimalToString(Price); }
            }
            set
            {
                decimal price;

                if (decimal.TryParse(value, out price))
                { this.Price = price; }
            }
        }

        public decimal Price
        {
            get
            {
                if (_priceNorm == null)
                { return 0m; }
                else
                { return _priceNorm.Price; }
            }
            set
            {
                if (!addPriceNorm(value, Multiply))
                { this._priceNorm.Price = value; }
            }
        }

        public double Multiply
        {
            get
            {
                if (_priceNorm == null)
                { return 0; }
                else
                { return _priceNorm.MultiAs; }
            }
            set
            {
                if (!addPriceNorm(Price, value))
                { this._priceNorm.MultiAs = value; }
            }
        }

        private int? getNextYM()
        {
            if (this._priceNorm == null)
            { return null; }
            else if ((bool)G.PriceNorm.QUERRY().EXIST.WHERE.AC(C.PriceNorm.YMTo).More.BV(this._priceNorm.YMTo).DO()[0].Value)
            {
                return (int)G.PriceNorm.QUERRY()
                        .GET
                        .C(C.PriceNorm.YMFrom)
                        .Min(C.PriceNorm.YMFrom).By(C.PriceNorm.YMFrom)
                        .WHERE
                        .AC(C.PriceNorm.YMFrom).More.BV(this._priceNorm.YMTo)
                        .AND.NOT.ID((this._priceNorm == null ? 0 : this._priceNorm.ID))
                        .DO()[0].Value;
            }
            else
            { return null; }
        }

        private bool getNextExist()
        {
            int ym;
            if (this._priceNorm == null || this._priceNorm.YMTo == 0)
            { ym = DateControl_Class.SelectMonth; }
            else
            { ym = this._priceNorm.YMTo; }

            return (bool)G.PriceNorm.QUERRY()
                           .EXIST
                           .Min(C.PriceNorm.YMFrom).By(C.PriceNorm.Pollution, C.PriceNorm.ResolutionClarify)
                           .WHERE
                               .AC(C.PriceNorm.YMFrom).More.BV(ym)
                           .AND
                               .AC(C.PriceNorm.Pollution).EQUI.BV(_pollutionID)
                           .AND
                               .AC(C.PriceNorm.ResolutionClarify).EQUI.BV(_resolutionClarifyID)
                           .AND
                               .NOT.ID((this._priceNorm == null ? 0 : this._priceNorm.ID))
                           .DO()[0].Value;
        }

        private void addItem(decimal price, double multiply, int ymFrom, int ymTo)
        {
            if ((bool)G.PriceNorm.QUERRY().ADD
                           .C(C.PriceNorm.Price, price)
                           .C(C.PriceNorm.MultiAs, multiply)
                           .C(C.PriceNorm.Pollution, _pollutionID)
                           .C(C.PriceNorm.ResolutionClarify, _resolutionClarifyID)
                           .C(C.PriceNorm.YMFrom, ymFrom)
                           .C(C.PriceNorm.YMTo, ymTo)
                           .DO()[0].Value)
            {
                var id = G.PriceNorm.Rows.GetID(G.PriceNorm.Rows.Count - 1);

                _priceNorm = Helpers.LogicHelper.PriceNormLogic.FirstOrDefault(id);
            }
            else
            { throw new Exception("ошибка добавления записи в таблицу PriceNorm"); }
        }

        private bool addPriceNorm(decimal price, double multiply)
        {
            if (price == Price && multiply == Multiply)
            { return true; }

            if (_priceNorm == null)
            {
                var ymNext = getNextYM();

                addItem(price, multiply, DateControl_Class.SelectMonth, (ymNext.HasValue ? ymNext.Value : 0));

                return true;
            }
            else if (this._priceNorm.YMFrom < DateControl_Class.SelectMonth && this._priceNorm.YMTo < DateControl_Class.SelectMonth)
            {
                //Есть-ли в переди по времени другое значение
                var existYM = getNextExist();

                if (!existYM)
                {
                    this._priceNorm.YMTo = DateControl_Class.SelectMonth - 1;

                    addItem(price, multiply, DateControl_Class.SelectMonth, 0);

                    return true;
                }
            }

            return false;
        }
    }
}