using sw.orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sw.test
{
    class Program
    {

        static void Main(string[] args)
        {

            //lambda表达式
            List<Test> tests1 = TestService.GetModelList(m => Const.TEMP.Equals(m.Name));

            List<Test> tests2 = TestService.GetModelList(m => m.Name.Contains(Const.TEMP));

            List<Test> tests3 = TestService.GetModelList(m => m.Name == "swtest".Trim());

            bool isExist = TestService.Exists(m => m.ID.Equals("86f72497-7d10-4e92-a0b5-14b7c3d4eb23") && m.Name == "sw11test");

            List<Test> tests5 = TestService.GetAllList();

            Test tests6 = TestService.GetModel(m => m.Name.Contains("swte344st"));



            int sumCount = TestService.GetCount(m => m.Name.Contains("sw"));


            List<string> names = new List<string>();
            names.Add("swtest");


            List<Test> tests10 = TestService.GetModelList(m => m.IsDel == false || ((m.Name.Equals("swtest")) || m.Creater.Equals("老沈879".Trim())));

            List<Test> tests11 = TestService.GetTopModelList(m => m.CreateTime > DateTime.Now.AddMinutes(-5), m => m.CreateTime, AscOrDesc.Desc, 4);


            Console.WriteLine("执行结束.");

            Console.ReadKey();
        }
    }
}
