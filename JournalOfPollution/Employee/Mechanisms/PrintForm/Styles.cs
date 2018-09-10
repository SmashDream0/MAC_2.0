
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAC_2.PrintForm
{
    public class Styles
    {
        public Styles(IWorkbook book)
        { this.book = book; }
        IWorkbook book;
        #region styles

        /// <summary>Border {RLTB}/ Alignment {CC}/ Font {Times New Roman 10}/ Wrap</summary>
        public static ICellStyle s_RLTB_CC_T10_W;
        /// <summary>Border {RLTB}/ Alignment {CC}/ Font {Times New Roman 9}/ Wrap</summary>
        public static ICellStyle s_RLTB_CC_T9_W;
        /// <summary>Border {RLTB}/ Alignment {LC}/ Font {Times New Roman 10}/ Wrap</summary>
        public static ICellStyle s_RLTB_LC_T10_W;
        /// <summary>Border {RLTB}/ Alignment {LC}/ Font {Times New Roman 9}/ Wrap</summary>
        public static ICellStyle s_RLTB_LC_T9_W;
        /// <summary>Border {RLTB}/ Alignment {CC}/ Font {Times New Roman 6}/ Wrap</summary>
        public static ICellStyle s_RLTB_CC_T6_W;
        /// <summary>Border {RLTB}/ Alignment {RC}/ Font {Times New Roman 10}/ Wrap</summary>
        public static ICellStyle s_RLTB_RC_T10_W;
        /// <summary>Border {RLTB}/ Alignment {RC}/ Font {Times New Roman 10}/ Wrap/ Bold</summary>
        public static ICellStyle s_RLTB_RC_T10_W_B;

        public virtual bool CreateStyle()
        {
            s_RLTB_CC_T10_W = book.CreateCellStyle();
            s_RLTB_CC_T9_W = book.CreateCellStyle();
            s_RLTB_LC_T10_W = book.CreateCellStyle();
            s_RLTB_LC_T9_W = book.CreateCellStyle();
            s_RLTB_CC_T6_W = book.CreateCellStyle();
            s_RLTB_RC_T10_W = book.CreateCellStyle();
            s_RLTB_RC_T10_W_B = book.CreateCellStyle();

            IFont font10 = book.CreateFont();
            font10.FontName = "Times New Roman";
            font10.FontHeightInPoints = (short)10;

            IFont font9 = book.CreateFont();
            font9.FontName = "Times New Roman";
            font9.FontHeightInPoints = (short)9;

            IFont font6 = book.CreateFont();
            font6.FontName = "Times New Roman";
            font6.FontHeightInPoints = (short)6;

            IFont font10_B = book.CreateFont();
            font10_B.FontName = "Times New Roman";
            font10_B.FontHeightInPoints = (short)10;
            font10_B.IsBold = true;

            s_RLTB_CC_T10_W.SetFont(font10);
            s_RLTB_CC_T9_W.SetFont(font9);
            s_RLTB_LC_T10_W.SetFont(font10);
            s_RLTB_LC_T9_W.SetFont(font9);
            s_RLTB_CC_T6_W.SetFont(font6);
            s_RLTB_RC_T10_W.SetFont(font10);
            s_RLTB_RC_T10_W_B.SetFont(font10_B);

            s_RLTB_CC_T6_W.Alignment = s_RLTB_CC_T10_W.Alignment = s_RLTB_CC_T9_W.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            s_RLTB_LC_T10_W.Alignment =
            s_RLTB_LC_T9_W.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            s_RLTB_RC_T10_W.Alignment =
            s_RLTB_RC_T10_W_B.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Right;

            s_RLTB_CC_T9_W.VerticalAlignment =
            s_RLTB_CC_T6_W.VerticalAlignment =
            s_RLTB_RC_T10_W_B.VerticalAlignment =
            s_RLTB_RC_T10_W.VerticalAlignment =
            s_RLTB_LC_T10_W.VerticalAlignment =
            s_RLTB_CC_T10_W.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

            s_RLTB_CC_T6_W.WrapText = s_RLTB_LC_T9_W.WrapText = s_RLTB_RC_T10_W.WrapText = s_RLTB_LC_T10_W.WrapText = s_RLTB_CC_T10_W.WrapText = true;

            s_RLTB_RC_T10_W_B.BorderRight = s_RLTB_RC_T10_W_B.BorderLeft = s_RLTB_RC_T10_W_B.BorderTop = s_RLTB_RC_T10_W_B.BorderBottom =
            s_RLTB_CC_T9_W.BorderRight = s_RLTB_CC_T9_W.BorderLeft = s_RLTB_CC_T9_W.BorderTop = s_RLTB_CC_T9_W.BorderBottom =
            s_RLTB_RC_T10_W.BorderRight = s_RLTB_RC_T10_W.BorderLeft = s_RLTB_RC_T10_W.BorderTop = s_RLTB_RC_T10_W.BorderBottom =
            s_RLTB_LC_T10_W.BorderRight = s_RLTB_LC_T10_W.BorderLeft = s_RLTB_LC_T10_W.BorderTop = s_RLTB_LC_T10_W.BorderBottom =
            s_RLTB_LC_T9_W.BorderRight = s_RLTB_LC_T9_W.BorderLeft = s_RLTB_LC_T9_W.BorderTop = s_RLTB_LC_T9_W.BorderBottom =
            s_RLTB_CC_T6_W.BorderRight = s_RLTB_CC_T6_W.BorderLeft = s_RLTB_CC_T6_W.BorderTop = s_RLTB_CC_T6_W.BorderBottom =
            s_RLTB_CC_T10_W.BorderRight = s_RLTB_CC_T10_W.BorderLeft = s_RLTB_CC_T10_W.BorderTop = s_RLTB_CC_T10_W.BorderBottom = BorderStyle.Thin;

            return true;
        }

        #endregion
    }
}
