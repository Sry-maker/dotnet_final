using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using WebApplication2.Repository;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    // <summary>
    /// 设置路由
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "reservation")]

    public class ReservationController
    {
        public static string generateId()
        {
            string reservation_id = "";
            int yr = DateTime.Now.Year;
            int mon = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            string date = yr.ToString().Substring(2, 2) + mon.ToString() + day.ToString();
            if (date.Length == 4)
            {
                date = date.Insert(2, "0");
                date = date.Insert(4, "0");
            }
            else if (date.Length == 5 && mon < 10)
            {
                date = date.Insert(2, "0");
            }
            else if (date.Length == 5 && day < 10)
            {
                date = date.Insert(4, "0");
            }
            reservation_id = date + "0000";
            using (var reservationRepo = new ReservationRepository())
            {
                string? max_id = reservationRepo.Reservations.
                    Where(p => p.reservation_id.StartsWith(date)).
                    Select(p => p.reservation_id).
                    Max();
                if (max_id != null)
                {
                    reservation_id = (long.Parse(max_id) + 1).ToString();
                }
            }
            return reservation_id;
        }

        /// <summary>
        /// 获取所有可预约餐桌
        /// </summary>
        /// <param name="customer_id"></param>
        /// <param name="reservation_date">yyyy-mm-dd</param>
        ///<param name="start_time">hh24:mi</param>
        ///<param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        public List<DiningTable> getAllUnreservedTable(string reservation_date, string start_time, int num)
        {
            var reservationRepo = new ReservationRepository();

            var usedList = reservationRepo.Reservations.
                        Where(p => p.reservation_date.Equals(DateTime.Parse(reservation_date)) && p.start_time.Equals(start_time) && p.state==0)
                        .Select(p=>p.table_id).ToList();

            return reservationRepo.Tables.Where(p => p.table_id.StartsWith("r") && p.capacity >= num)
                .Where(p => !usedList.Contains(p.table_id))
                .OrderBy(p => p.table_id)
                .ToList();

        }

        /// <summary>
        /// 预约餐桌
        /// </summary>
        /// <param name="customer_id"></param>
        /// <param name="reservation_date">yyyy-mm-dd</param>
        ///<param name="start_time">hh24:mi</param>
        ///<param name="end_time"></param>
        ///<param name="table_id"></param>
        /// <returns></returns>
        /// <remarks>
        /// 传入：日期，顾客id，餐桌id，开始时间，结束时间
        /// 返回：1-预约成功
        /// 2-预约已满
        /// 3-当日已经预约
        /// 4-用户不存在
        /// </remarks>
        [HttpPost]
        public int reserveTable(string customer_id,string reservation_date,string start_time,string end_time,string table_id)
        {
            using(var customerRepo = new CustomerRepository())
            {
                if (customerRepo.Customers.Find(customer_id) == null) return 4;
            }
            var reservationRepo = new ReservationRepository();
            if (reservationRepo.Reservations.Where(p => p.reservation_date.Equals(DateTime.Parse(reservation_date))
                && p.start_time.Equals(start_time) && p.customer_id.Equals(customer_id))
                .Count() > 0) return 3;
            if (getAllUnreservedTable(reservation_date, start_time, 0).Select(p => p.table_id).Contains(table_id))
            {
                Reservation reservation = new Reservation();
                reservation.start_time = start_time;
                reservation.end_time = end_time;
                reservation.table_id = table_id;
                reservation.reservation_date = DateTime.Parse(reservation_date);
                reservation.state = 0;
                reservation.customer_id = customer_id;
                reservation.reservation_id = generateId();
                reservationRepo.Add(reservation);
                reservationRepo.SaveChanges();
                return 1;
            }
            return 2 ;
        }

        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="reservation_id"></param>
        /// <returns></returns>
        /// <remarks>
        /// 需传入reservation_id  返回：0-失败 1-删除成功
        /// </remarks>
        [HttpPost]
        public ActionResult<int> cancelReservation(string reservation_id)
        {
            ReservationRepository reservationRepo = new ReservationRepository();
            var reservation = reservationRepo.Reservations.Find(reservation_id);
            if (reservation != null && reservation.state == 0 )
            {
                if(reservation.reservation_date > DateTime.Now)
                {
                    reservation.state = 2;
                    reservationRepo.SaveChanges();
                    return 1;
                }
            }
            return 0;
        }
    }
}
