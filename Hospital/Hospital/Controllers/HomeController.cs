using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hospital.Models;

namespace Hospital.Controllers
{
    public class HomeController : Controller
    {
        HospitalDBEntities db = new HospitalDBEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.zaizhi =db.Doctor.Where(n=>n.Ystate=="在职").Count();
            ViewBag.lizhi= db.Doctor.Where(n => n.Ystate == "已离职").Count();
            ViewBag.xiujia = db.Doctor.Where(n => n.Ystate == "休假").Count();
            ViewBag.zhuyuan = db.Zhuyuan.Where(n => n.state == "已入院").Count();
            ViewBag.meng = db.Guahao.ToList().Count();//挂号人数
            ViewBag.h = db.Guahao.Where(n=>n.Gstate=="0").ToList();//今天挂号
            ViewBag.k = db.Bumen.ToList().Count;
            ViewBag.z = db.Guahao.Where(n => n.Gtime.Month == DateTime.Now.Month).ToList().Count();
          var y = db.Doctor.ToList();
            return View(y);
        }
        //登录的方法
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Login login)
        {
            var p = db.Login.FirstOrDefault(n => n.Account == login.Account && n.Password == login.Password && n.Doctor.Ystate!= "已离职");
            if (p!=null)
            {
                Session["name"] = p.Doctor.Yname;//医生姓名
                Session["zhiwu"] = p.Zhiwu;//医生职务
                Session["Yid"] = p.YsID;//医生ID
                return Redirect("/Home/Index");
            }
            else
            {
                return Content("<script> alert('登录失败！账号或密码错误！');window.location.href='/Home/Login';</script > ");
            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session.Remove("name");
            return RedirectToAction("Login");
        }
        /// <summary>
        /// 用户管理模块
        /// </summary>
        /// <returns></returns>
        public ActionResult Yhu()
        {
            var p = db.Login.ToList();
            ViewBag.li = db.Doctor.ToList();
            return View(p);
        }
        [HttpPost]
        public ActionResult Yhu(Login l)
        {
            var p = db.Login.ToList();
            ViewBag.li = db.Doctor.ToList();         
            db.Login.Add(l);           
            if (db.SaveChanges()>0)
            {
                return RedirectToRoute(p);
            }
            else
            {
                return Content("<script> alert('添加失败请重试！');window.location.href='/Home/Yhu';</script > ");
            }
           
        }
        /// <summary>
        /// 用户删除方法
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public ActionResult Del(int ac)
        {
            var p = db.Login.Find(ac);
            db.Login.Remove(p);
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
        /// 修改方法
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Account"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public ActionResult Xiu(int ID ,string Account,string Password)
        {
            var p= db.Login.Find(ID);
            p.Account = Account;
            p.Password = Password;
            if (db.SaveChanges() > 0)
            {
                return Content("yes");
            }
            else
            {
                return Content("no");
            }
        }

    }
}