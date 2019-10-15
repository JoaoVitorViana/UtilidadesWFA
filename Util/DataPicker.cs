using System;
using System.Windows.Forms;

namespace UtilidadesWFA.Util
{
    public class DataPicker
    {
        public static void ValueChanged(DateTimePicker dateTime)
        {
            if (dateTime.Value == DateTimePicker.MinimumDateTime)
            {
                dateTime.Value = DateTime.Now.AddDays(1);
                dateTime.Format = DateTimePickerFormat.Custom;
                dateTime.CustomFormat = " ";
            }
            else
            {
                dateTime.Format = DateTimePickerFormat.Short;
            }
        }

        public static void ValueChangedMesAno(DateTimePicker dateTime)
        {
            if (dateTime.Value == DateTimePicker.MinimumDateTime)
            {
                dateTime.Value = DateTime.Now.AddDays(1);
                dateTime.Format = DateTimePickerFormat.Custom;
                dateTime.CustomFormat = " ";
            }
            else
            {
                dateTime.Format = DateTimePickerFormat.Custom;
                dateTime.CustomFormat = "MM/yyyy";
            }
        }
    }
}
