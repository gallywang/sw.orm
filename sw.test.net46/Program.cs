using sw.orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sw.test.net46
{
    class Program
    {
        static DBClient Temp = SWClient.Initialize("Data Source = .; Initial Catalog = Temp; User ID = sa; Pwd = 1992Sxy1113", DBType.SQLServer);
        static void Main(string[] args)
        {
            List<Test> tests = new List<Test>();

            string id = Guid.NewGuid().ToString();

            Test test = new Test();
            test.ID = id;
            test.Name = "swtest";
            test.TestContent = "这是在测试879";
            test.Creater = "老沈879";
            test.CreateTime = DateTime.Now;
            test.Sort = 5;
            test.Number = 11;
            test.IsDel = true;
            Temp.Insert<Test>(test);

            test.TestContent = null;
            Temp.Update(test);

            //Test updateModel = new Test();
            //updateModel.Number = 35;
            //Temp.Update<Test>(m=>updateModel, m=>m.ID == id);

            List<Test> tests1 = Temp.GetModelList<Test>(m => m.TestContent == null, "Number,CreateTime", AscOrDesc.Desc);

            //tests.Add(test);

            //Test test2 = new Test();
            //test2.ID = Guid.NewGuid().ToString();
            //test2.Name = "swtest";
            //test2.TestContent = "这是在测试879";
            //test2.Creater = "老沈879";
            //test2.CreateTime = DateTime.Now;
            //test2.Sort = 5;
            //test2.Number = 22;
            //test2.IsDel = true;
            ////Temp.Insert<Test>(test);

            //tests.Add(test2);

            //Temp.Insert(tests);

            //List<string> ids = new List<string>();
            //ids.Add(test.ID);
            //ids.Add(test2.ID);
            //Temp.Delete<Test>(ids);

            //将某字段更新为空
            //Test updateModel = Temp.GetModel<Test>(m=>m.ID == test2.ID);
            //updateModel.CreateTime = null;
            //Temp.Update<Test>(updateModel);

            //修改
            //test.TestContent = "侧个鬼列";
            //Temp.Update<Test>(test);


            //sqlClient.CommitTran();
            //sqlClient.Dispose();

            //删除
            //sqlClient.Delete<Test>(test);

            //查询测试

            //lambda表达式
            //List<Test> tests1 = Temp.GetModelList<Test>(n => new Test { Name = "swtest" }, "");

            //List<Test> tests2 = Temp.GetModelList<Test>(m => m.Name.Contains("swtest".Trim()));

            //List<Test> tests21 = Temp.GetModelList<Test>(m => m.Name.Contains("swtest".ToLower()));

            //List<Test> tests22 = Temp.GetModelList<Test>(m => m.Name.Contains("swtest".ToLower()));

            //List<Test> tests3 = Temp.GetModelList<Test>(m => m.Name == "swtest".Trim());

            //bool isExist = Temp.Exists<Test>(m => m.ID.Equals("86f72497-7d10-4e92-a0b5-14b7c3d4eb23") && m.Name == "sw11test");

            //List<Test> tests4 = Temp.GetModelList<Test>(m => m.Name.Contains("swtest"), m => m.CreateTime, AscOrDesc.Desc, 10, 2);

            //List<Test> tests5 = Temp.GetAll<Test>();

            //Test tests6 = Temp.GetModel<Test>(m => m.Name.Contains("swte344st"));

            //int count = 0;

            //List<Test> tests7 = Temp.GetModelListWithCount<Test>(m => m.Name.Contains("swtest"), out count, m => m.CreateTime, AscOrDesc.Desc, 10, 2);


            //int sumCount = Temp.GetCount<Test>();

            //List<Test> tests8 = Temp.GetModelList<Test>(m => m.Name.Contains("swtest"), m => m.CreateTime, AscOrDesc.Desc, 4);

            //List<string> names = new List<string>();
            //names.Add("swtest");

            //List<Test> tests9 = Temp.GetModelList<Test>(m => m.Name.In(names), m => m.CreateTime, AscOrDesc.Desc, 4);

            //List<Test> tests10 = Temp.GetModelList<Test>(m => m.IsDel == false && ((m.Name.Equals("swtest")) || m.Creater.Equals("老沈879".Trim())));

            //DateTime? startTime = DateTime.Now.AddDays(-5);
            //DateTime? endTime = DateTime.Now.AddDays(1);

            //List<Test> tests11 = Temp.GetModelList<Test>(m => m.CreateTime > Convert.ToDateTime(startTime) && m.CreateTime < Convert.ToDateTime(endTime), m => m.CreateTime, AscOrDesc.Desc, 4);

            //List<Test> tests12 = Temp.GetModelList<Test>(m => m.Sort == 5);
        }
    }
}
