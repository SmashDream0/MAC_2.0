using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MAC_2.Employee.Mechanisms
{
    static class InstructionsMessage_Class
    {
        public static void LoadInstructions(Menu menu, data.ETypeInstruction type)
        {
            var StorageInstructions = T.StorageInstructions.CreateSubTable(false);
            StorageInstructions.QUERRY()
                .SHOW
                .WHERE
                    .C(C.StorageInstructions.UType, data.User<uint>(C.User.UType))
                    .AND
                    .C(C.StorageInstructions.TypeInstruction, (uint)type)
                .DO();

            if (StorageInstructions.Rows.Count > 0)
            {
                MenuItem MenuI = new MenuItem();

                WrapPanel WP = new WrapPanel();

                Image Im1 = new Image();
                Im1.Height = 15;
                Im1.Source=new BitmapImage(new Uri("pack://application:,,,/Resources/Instructions1.png"));//подгрузка картинки
                WP.Children.Add(Im1);

                TextBlock tb = new TextBlock();
                tb.Text = "Инструкции";
                WP.Children.Add(tb);

                Image Im2 = new Image();
                Im2.Height = 15;
                Im2.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Instructions2.png"));//подгрузка картинки
                WP.Children.Add(Im2);

                MenuI.Header = WP;
                string Message = string.Empty;
                menu.Items.Add(MenuI);
                for (int i = 0; i < StorageInstructions.Rows.Count; i++)
                {
                    string Path = Directory.GetCurrentDirectory().ToString() + StorageInstructions.Rows.Get<string>(i, C.StorageInstructions.Path);
                    if (File.Exists(Path))
                    {
                        MenuItem Mi = new MenuItem();
                        Mi.Header = StorageInstructions.Rows.Get<string>(i, C.StorageInstructions.Name);
                        Mi.Click += (sender, e) => { System.Diagnostics.Process.Start(Path); };
                        MenuI.Items.Add(Mi);
                    }
                    else
                    { Message = $"\"{StorageInstructions.Rows.Get<string>(i, C.StorageInstructions.Name)}\","; }
                }
                if (Message.Length > 0)
                { MessageBox.Show("Инструкции " + Message.Trim(',') + " не найдены"); }
            }
        }
        //static DataBase.ISTable NoticeMessage;
        //static int AllMessage;
        //public static void LoadMessages(Menu menu)
        //{
        //    if (AllMessage < T.NoticeMessage.DataSource.RowCount)
        //    {
        //        NoticeMessage = T.NoticeMessage.CreateSubTable(false);
        //        NoticeMessage.QUERRY()
        //            .SHOW
        //            .WHERE
        //                .C(C.NoticeMessage.User, data.UserID)
        //            .OR
        //                .C(C.NoticeMessage.UType, data.User<uint>(C.User.UType))
        //            .DO();
        //        MenuItem MenuI = new MenuItem();
        //        for (int i = 0; i < menu.Items.Count; i++)
        //            if (menu.Items.GetItemAt(i) is MenuItem)
        //                if (((MenuItem)menu.Items.GetItemAt(i)).Header.ToString().Contains("Новые сообщения"))
        //                    menu.Items.RemoveAt(i);
        //        if (NoticeMessage.Rows.Count > 0)
        //        {
        //            MenuI.Background = Brushes.Green;
        //            MenuI.Header = "Новые сообщения";
        //            menu.Items.Add(MenuI);
        //            for (int i = 0; i < NoticeMessage.Rows.Count; i++)
        //            {
        //                MenuItem Mi = new MenuItem();
        //                Mi.Header = NoticeMessage.Rows.Get<string>(i, C.NoticeMessage.Text).StringDivision(20);
        //                int Index = i;
        //                Mi.Click += (sender, e) =>
        //                {
        //                    if (MessageBox.Show(NoticeMessage.Rows.Get<string>(Index, C.NoticeMessage.Text), "Удалить сообщение?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //                    {
        //                        G.NoticeMessage.QUERRY()
        //                          .DELETE
        //                          .WHERE
        //                          .ID(NoticeMessage.Rows.GetID(Index))
        //                          .DO();
        //                        menu.Items.Remove(Mi);
        //                    }
        //                };
        //                MenuI.Items.Add(Mi);
        //            }
        //        }
        //        AllMessage = T.NoticeMessage.DataSource.RowCount;
        //    }
        //}
    }
}