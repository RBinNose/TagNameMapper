// See https://aka.ms/new-console-template for more information
IDisplay display = new Display();
display.Show1();
display.Show2("你好1", "你好2");
System.Console.WriteLine("---------------------");
display.Show2("你好1", "你好2").Show1();
System.Console.WriteLine("---------------------");
DisplayExtension.Show2(display,"你好1","你好2").Show2("你好3","你好4").Show2("你好5","你好6");


interface IDisplay
{
    void Show1();
}

class Display : IDisplay
{
    public void Show1()
    {
        Console.WriteLine("Show1");
    }
}

//拓展方法
static class DisplayExtension
{
    /// <summary>
    /// 显示str1+str2
    /// </summary>
    /// <param name="display">拓展方法的实例</param>
    /// <param name="str1">要显示的字符串1</param>
    /// <param name="str2">要显示的字符串2</param>
    /// <returns>返回当前对象</returns>
    public static IDisplay Show2(this IDisplay display, string str1,string str2)
    {
        Console.WriteLine(str1+str2);
        return display;
    }
}