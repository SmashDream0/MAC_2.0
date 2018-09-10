using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAC_2.Logic;

namespace MAC_2.Helpers
{
    public static class LogicHelper
    {
        static LogicHelper()
        { }

        private static Dictionary<DataBase.ITable, BaseLogic> _logics;

        public static NegotiationAssistantLogic NegotiationAssistantLogic = new NegotiationAssistantLogic();
        public static NormDocLogic NormDocLogic = new NormDocLogic();
        public static AdresLogic AdresLogic = new AdresLogic();
        public static DetailsObjectLogic DetailsObjectLogic = new DetailsObjectLogic();
        public static CalculationFormulaLogic CalculationFormulaLogic = new CalculationFormulaLogic();
        public static AccurateMeasurementLogic AccurateMeasurementLogic = new AccurateMeasurementLogic();
        public static CoefficientValueLogic CoefficientValueLogic = new CoefficientValueLogic();
        public static CoefficientLogic CoefficientLogic = new CoefficientLogic();
        public static ValueNormLogic ValueNormLogic = new ValueNormLogic();
        public static PriceNormLogic PriceNormLogic = new PriceNormLogic();
        public static SelectionWellLogic SelectionWellLogic = new SelectionWellLogic();
        public static UnitsLogic UnitsLogic = new UnitsLogic();
        public static AccreditLogic AccreditLogic = new AccreditLogic();
        public static ClientsLogic ClientsLogic = new ClientsLogic();
        public static DeclarationLogic DeclarationLogic = new DeclarationLogic();
        public static DeclarationValueLogic DeclarationValueLogic = new DeclarationValueLogic();
        public static DetailsClientLogic DetailsClientLogic = new DetailsClientLogic();
        public static ObjectFromResolutionLogic ObjectFromResolutionLogic = new ObjectFromResolutionLogic();
        public static ObjecteLogic ObjecteLogic = new ObjecteLogic();
        public static PeiodLogic PeiodLogic = new PeiodLogic();
        public static PollutionLogic PollutionLogic = new PollutionLogic();
        public static RatioSignerLogic RatioSignerLogic = new RatioSignerLogic();
        public static ResolutionClarifyLogic ResolutionClarifyLogic = new ResolutionClarifyLogic();
        public static SampleLogic SampleLogic = new SampleLogic();
        public static ValuesSelectionLogic ValuesSelectionLogic = new ValuesSelectionLogic();
        public static WellLogic WellLogic = new WellLogic();
        public static WorkerLogic WorkerLogic = new WorkerLogic();
        public static ResolutionLogic ResolutionLogic = new ResolutionLogic();
        public static UnitLogic UnitLogic = new UnitLogic();
        public static VolumeLogic VolumeLogic = new VolumeLogic();

        /// <summary>
        /// Очистить все кеши
        /// </summary>
        public static void ClearCacheAll()
        {
            foreach (var logic in _logics.Values)
            {
                logic.Cache.Clear();
            }
        }

        /// <summary>
        /// Очистить все кеши
        /// </summary>
        public static void ClearQuerryCacheAll()
        {
            foreach (var logic in _logics.Values)
            {
                logic.Cache.ClearQuerry();
            }
        }

        public static BaseLogic TryGetLogic(DataBase.ITable table)
        {
            BaseLogic logic;

            _logics.TryGetValue(table, out logic);

            return logic;
        }

        public static void InitLogics()
        {
            var fields = typeof(LogicHelper).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            var logics = fields.Where(x => typeof(BaseLogic).IsAssignableFrom(x.FieldType)).Select(x => x.GetValue(null)).Cast<BaseLogic>().ToArray();

            _logics = new Dictionary<DataBase.ITable, BaseLogic>();

            foreach (var logic in logics)
            {
                _logics.Add(logic.Table, logic);

                logic.Table.Rows.AfterAddRow += (table, id) => clearCache(table.Parent);
                logic.Table.Rows.SetValue += (table, values) => clearCache(table);
                logic.Table.Rows.AfterChangeRowStatus += (table, record) => clearCache(table);
            }
        }

        static void clearCache(DataBase.ITable table)
        {
            ClearQuerryCacheAll();
        }
    }
}
