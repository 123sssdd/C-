using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hospital.Models;

namespace Hospital.Controllers
{
    public class ZhuController : Controller
    {
        HospitalDBEntities db = new HospitalDBEntities();
        // GET: Zhu
        public ActionResult Index()
        {
            var p = db.Zhuyuan.Where(n => n.state == "未入院").ToList();
            ViewBag.h= db.Zhuyuan.Where(n => n.state == "已入院").ToList();
            return View(p);
        }
        /// <summary>
        /// 入院登记
        /// </summary>
        /// <param name="chuang"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public ActionResult Tian(int Zhuid, int chuang,decimal money)
        {
            ViewBag.h = db.Zhuyuan.Where(n => n.state == "已入院").ToList();
            var zh = db.Zhuyuan.Find(Zhuid);
            zh.Chuang = chuang;
            zh.money = money;
            zh.riqi = DateTime.Now;
            zh.state = "已入院";
            if (db.SaveChanges()>0)
            {
                return Content("yes");
            }
            else
            {
                return Content("no");
            }
            
        }
        /// <summary>
        /// 出院结算
        /// </summary>
        /// <param name="Zhuid"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public ActionResult Chu( int Zhuid,decimal money)
        {
            var zh = db.Zhuyuan.Find(Zhuid);
            zh.moneyz = money;
            zh.riqic = DateTime.Now;
            zh.state = "已出院";
            if (db.SaveChanges() > 0)
            {
                return Content("yes");
            }
            else
            {
                return Content("no");
            }
        }
        /// <summary>
        /// 住院处方划价
        /// </summary>
        /// <returns></returns>
        public ActionResult Zhuchu()
        {
            ViewBag.Yao = db.Medicine.Where(n => n.Mguoqi > DateTime.Now);
            var p= db.Zhuyuan.Where(n => n.state == "已入院").ToList();
            return View(p);
        }
        /// <summary>
        /// 添加药品到处方表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Buy(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;//贪婪加载
            var data = db.Medicine.SingleOrDefault(n => n.medicineID == id);
            return Json(new { code = 0, data = data });
        }
        /// <summary>
        /// 处方表提交方法
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult submit(List<test> list)
        {
            List<Morder> moList = new List<Morder>();
            foreach (var item in list)
            {
                //接收传过来的值（药品ID，购买的药品数量添加到订单表）
                Morder morder = new Morder { medicineID = item.ids, Mshu = item.num, Gid = item.gid, state = item.state, zhifu = item.zhifu, mhz = item.mhz };
                //改变药品表的药品数量
                var p = db.Medicine.Find(item.ids);
                if (p != null)
                {
                    p.Mshu = p.Mshu - item.num;
                }
                moList.Add(morder);
            }
            db.Morder.AddRange(moList);
            db.SaveChanges();
            return Json(new { code = 0 });
        }
        /// <summary>
        /// 住院患者库
        /// </summary>
        /// <returns></returns>
        public ActionResult Huan()
        {
            var h = db.Zhuyuan.ToList();
            return View(h);
        }
        [HttpPost]
        public ActionResult Huan(string name)
        {
            var h = db.Zhuyuan.Where(n=>n.Guahao.Gname.Contains(name)).ToList();
            return View(h);
        }
    }
    public class test1
    {
        public int ids { get; set; }
        public int num { get; set; }
        public int gid { get; set; }//挂号id
        public string state { get; set; }
        public string zhifu { get; set; }
        public string mhz { get; set; }
    }
}