namespace NetSync_WinDesktop
{
    class FileDescript
    {
        //Значение флагов модификации:
        // 0 - файл не был изменен
        // 1 - было изменено содержимое файла
        // 2 - файл был создан
        // 3 - файл был удален  

        public string ElementName { get; set; }
        public int ModificationFlag { get; set; }

    }
}
