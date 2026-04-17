namespace TestTask.Utils
{
    public static class MonthHelper
    {
        public static int GetMonthOrder(string month)
        {
            return month?.ToLower() switch
            {
                "january" or "январь" => 0,
                "february" or "февраль" => 1,
                "march" or "март" => 2,
                "april" or "апрель" => 3,
                "may" or "май" => 4,
                "june" or "июнь" => 5,
                "july" or "июль" => 6,
                "august" or "август" => 7,
                "september" or "сентябрь" => 8,
                "october" or "октябрь" => 9,
                "november" or "ноябрь" => 10,
                "december" or "декабрь" => 11,
                _ => 99
            };
        }

        public static string GetMonthRu(string month)
        {
            return month?.ToLower() switch
            {
                "january" => "Январь",
                "february" => "Февраль",
                "march" => "Март",
                "april" => "Апрель",
                "may" => "Май",
                "june" => "Июнь",
                "july" => "Июль",
                "august" => "Август",
                "september" => "Сентябрь",
                "october" => "Октябрь",
                "november" => "Ноябрь",
                "december" => "Декабрь",
                _ => month
            };
        }
    }
}