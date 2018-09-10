using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Employee.Mechanisms;

namespace MAC_2.Employee.Windows.Norm
{
    public class NormUnitValue : NormBaseValue
    {
        public NormUnitValue(uint id)
        {
            _valueNorm = Helpers.LogicHelper.ValueNormLogic.FirstModel(id);

            this._pollutionID = _valueNorm.PollutionID;
            this._resolutionClarifyID = _valueNorm.ResolutionClarifyID;
            this._unitID = _valueNorm.UnitID;
        }

        public NormUnitValue(uint pollutionID, uint resolutionClarifyID, uint unitID)
        {
            this._pollutionID = pollutionID;
            this._resolutionClarifyID = resolutionClarifyID;
            this._unitID = unitID;
        }

        public NormUnitValue(uint pollutionID, uint resolutionClarifyID)
            : this(pollutionID, resolutionClarifyID, 0)
        { }

        private readonly uint _pollutionID;
        private readonly uint _resolutionClarifyID;
        private readonly uint _unitID;
        private const string _splitValue = "-";

        private Model.ValueNorm _valueNorm;

        public override string Value
        {
            get
            {
                if (From == 0 && To == 0)
                { return String.Empty; }
                else
                { return $"{(From > 0 ? $"{gecimalToString(From)} {_splitValue} " : String.Empty)}{gecimalToString(To)}"; }
            }
            set
            { setValue(value); }
        }

        private void setValue(string value)
        {
            value = value.Replace('.', ',');
            var split = value.Split(new[] { _splitValue }, StringSplitOptions.RemoveEmptyEntries);

            decimal from = 0, to = 0;

            if (split.Length > 1)
            {
                tryGetValue(split[0], (result) => from = result);
                tryGetValue(split[1], (result) => to = result);
            }
            else
            {
                tryGetValue(split[0], (result) => to = result);
            }

            if (from <= to)
            {
                this.From = from;
                this.To = to;
            }
        }

        private static void tryGetValue(string value, Action<decimal> setFunc)
        {
            decimal result;

            if (decimal.TryParse(value, out result))
            { setFunc(result); }
        }

        public decimal From
        {
            get
            {
                if (_valueNorm == null)
                { return 0m; }
                else
                { return _valueNorm.From; }
            }
            set
            {
                if (!addValueNorm(value, To, Multiply))
                { this._valueNorm.From = value; }
            }
        }

        public decimal To
        {
            get
            {
                if (_valueNorm == null)
                { return 0m; }
                else
                { return _valueNorm.To; }
            }
            set
            {
                if (!addValueNorm(From, value, Multiply))
                { this._valueNorm.To = value; }
            }
        }

        public decimal Multiply
        {
            get
            {
                if (_valueNorm == null)
                { return 0m; }
                else
                { return _valueNorm.Multiplier; }
            }
            set
            {
                if (!addValueNorm(From, To, value))
                { this._valueNorm.Multiplier = value; }
            }
        }

        private int? getNextYM()
        {
            if (this._valueNorm == null)
            { return null; }
            else if ((bool)G.ValueNorm.QUERRY().EXIST.WHERE.AC(C.ValueNorm.YMTo).More.BV(this._valueNorm.YMTo).DO()[0].Value)
            {
                return (int)G.ValueNorm.QUERRY()
                        .GET
                        .C(C.ValueNorm.YMFrom)
                        .Min(C.ValueNorm.YMFrom).By(C.ValueNorm.YMFrom)
                        .WHERE
                            .AC(C.ValueNorm.YMFrom).More.BV(this._valueNorm.YMTo)
                        .AND
                            .AC(C.ValueNorm.Unit).EQUI.BV(_unitID)
                        .AND
                            .AC(C.ValueNorm.Pollution).EQUI.BV(_pollutionID)
                        .AND
                            .NOT.ID((this._valueNorm == null ? 0 : this._valueNorm.ID))
                        .DO()[0].Value;
            }
            else
            { return null; }
        }

        private bool getNextExist()
        {
            int ym;
            if (this._valueNorm == null || this._valueNorm.YMTo == 0)
            { ym = DateControl_Class.SelectMonth; }
            else
            { ym = this._valueNorm.YMTo; }

            return (bool)G.ValueNorm.QUERRY()
                      .EXIST
                      .Min(C.ValueNorm.YMFrom).By(C.ValueNorm.Unit, C.ValueNorm.Pollution, C.ValueNorm.ResolutionClarify)
                      .WHERE
                          .AC(C.ValueNorm.YMFrom).More.BV(ym)
                      .AND
                          .AC(C.ValueNorm.Unit).EQUI.BV(_unitID)
                      .AND
                          .AC(C.ValueNorm.Pollution).EQUI.BV(_pollutionID)
                      .AND
                          .AC(C.ValueNorm.ResolutionClarify).EQUI.BV(_resolutionClarifyID)
                      .AND
                          .NOT.ID((this._valueNorm == null ? 0 : this._valueNorm.ID))
                      .DO()[0].Value;
        }

        private void addItem(decimal from, decimal to, decimal multiply, int ymFrom, int ymTo)
        {
            if ((bool)G.ValueNorm.QUERRY().ADD
                           .C(C.ValueNorm.From, from)
                           .C(C.ValueNorm.To, to)
                           .C(C.ValueNorm.Multiplier, multiply)
                           .C(C.ValueNorm.Pollution, _pollutionID)
                           .C(C.ValueNorm.Unit, _unitID)
                           .C(C.ValueNorm.ResolutionClarify, _resolutionClarifyID)
                           .C(C.ValueNorm.YMFrom, ymFrom)
                           .C(C.ValueNorm.YMTo, ymTo)
                           .DO()[0].Value)
            {
                var id = G.ValueNorm.Rows.GetID(G.ValueNorm.Rows.Count - 1);

                _valueNorm = Helpers.LogicHelper.ValueNormLogic.FirstOrDefault(id);
            }
            else
            { throw new Exception("ошибка добавления записи в таблицу ValueNorm"); }
        }

        private bool addValueNorm(decimal from, decimal to, decimal multiply)
        {
            if (from == From && to == To && multiply == Multiply)
            { return true; }

            if (_valueNorm == null)
            {
                var ymNext = getNextYM();

                addItem(from, to, multiply, DateControl_Class.SelectMonth, (ymNext.HasValue ? ymNext.Value : 0));

                return true;
            }
            else if (this._valueNorm.YMFrom < DateControl_Class.SelectMonth && this._valueNorm.YMTo < DateControl_Class.SelectMonth)
            {
                //Есть-ли в переди по времени другое значение
                var existYM = getNextExist();

                if (!existYM)
                {
                    this._valueNorm.YMTo = DateControl_Class.SelectMonth - 1;

                    addItem(from, to, multiply, DateControl_Class.SelectMonth, 0);

                    return true;
                }
            }

            return false;
        }
    }
}