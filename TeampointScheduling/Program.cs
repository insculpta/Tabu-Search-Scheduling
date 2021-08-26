using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TeampointScheduling;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions; // for Regex.Split

namespace TeampointScheduling
{
    public class Person
    {
        public int personID;
        public int max_minutes;
        public int max_miles;
        public int[] p_tags; //qualification tags
        public int[] dayoff;
        public int init_pos; //initial position ID

     
    }

    public class Job 
    {
        public int jobID;
        public int priority;
        public int duration;       
        public int[] tags;
        public int job_dates;
        public int window_num;
        public int win_start;
        public int win_end;
        public int type; // 1:work 2:drive 3:break 4:buffer
        public int calcutate;

    }

    public class Staff  // in use
    {
        public int personID;
        public int[] tags;
        public string[] work_dates;
        public int[] work_str;
        public int[] work_end;
        public int[] max_minutes;
        public int max_miles;

        public int init_pos; //initial position ID

    }
    public class Jobs  //in use
    {
        public int jobID;
        public int priority;
        public int duration;
        public int[] tags;
        public string[] job_dates;
        public int[] win_str;
        public int[] win_end;

        /*
        public Jobs(int jobID, int priority, int duration, string[] job_dates, int[] win_str, int[] win_end, int[] tags)
        {
            this.jobID = jobID;
            this.priority = priority;
            this.duration = duration;           
            this.job_dates = job_dates;
            this.win_str = win_str;
            this.win_end = win_end;
            this.tags = tags;
        }
        */
        //public string windows;
        //public int window_num;
        //public int type; // 1:work 2:drive 3:break 4:buffer
        //public int calcutate;

    }

    public class Duty //in use
    {
        public string date;
        public List<Jobs> dutyset;
    }

    public class DaySchedule 
    {
        public int personID;
        public int[] p_tags;
        public List<Job> duty;
        public int dutyTime; // total working time
        public int drivingTime; // total driving distance       
        public int dutyNum; //total work duty
        public int endTime; //current all the duty end time        
    }

    public class Schedule // in use
    {
        public int personID;
        public List<Duty> duty;
        public int[] duty_str; //the real str time of each duty
        public int[] duty_end; //the real end time of each duty
        public int totalwork;
        public int totaldrive;

    }

    public class assign
    { 
        public Jobs job;
        public int jobID;
        public int job_str; 
        public int job_end;
        public int totalwork;
        public int totaldrive;

    }


    public class available
    {
        public int str;
        public int end;
        public int duration;
        
    }

    public class location
    {
        public int from;
        public int to;
        public int duration;
    }
    
    class Program
    {
        public static void ReadFile(List<int> personID, List<int> max_minutes, List<int> max_miles, List<string> tags, List<string> dayoff)
        {

            string[] operative;
            if (File.Exists(@"testoperative.csv"))  
            {
                //Console.WriteLine("The file exists!");
                StreamReader sr = new StreamReader(@"testoperative.csv");
                while (!sr.EndOfStream)
                {
                    operative = sr.ReadLine().Split(',');
                    personID.Add(int.Parse(operative[0]));
                    max_minutes.Add(int.Parse(operative[1]));
                    max_miles.Add(int.Parse(operative[2]));
                    tags.Add(operative[3]);
                    dayoff.Add(operative[4]);
                }
            }
            else
            {
                Console.WriteLine("The file doesn\'t exists !");
            }

        }
        public static void PrintStr(string i) 
        {
            Console.WriteLine(i);
        }
        public static void Print(int i)
        {
            Console.WriteLine(i);
        }
        static void Main(string[] args)
        {
            
            
            
            //operative data
            List<int> personid = new List<int>();
            List<int> max_minutes = new List<int>();
            List<int> max_miles = new List<int>();
            List<string> dayoff = new List<string>();
            List<int> worker_tags = new List<int>();
            
            
            // read from excel
            //ReadFile(personID, max_minutes, max_miles, tags, dayoff);



            void Show(string[] entries)
            {
                Console.WriteLine($"The return value contains these {entries.Length} elements:");
                foreach (string entry in entries)
                {
                    //Console.Write($"<{entry}>");
                    Console.WriteLine(entry);
                }
                Console.Write("\n\n");
            }

            string testsplit = "[{\"date\":\"2021-06-01\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-02\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-03\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-04\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-05\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-06\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-07\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-08\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-09\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-10\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-11\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-12\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-13\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-14\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-15\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-16\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-17\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-18\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-19\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-20\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-21\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-22\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-23\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-24\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-25\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-26\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-27\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-28\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-29\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-06-30\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-01\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-02\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-03\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-04\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-05\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-06\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-07\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-08\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-09\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-10\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-11\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-12\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-13\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-14\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-15\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-16\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-17\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-18\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-19\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-20\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-21\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-22\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-23\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-24\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-25\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-26\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-27\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-28\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-29\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-30\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-07-31\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-01\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-02\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-03\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-04\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-05\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-06\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-07\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-08\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-09\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-10\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-11\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-12\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-13\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-14\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-15\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-16\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-17\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-18\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-19\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-20\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-21\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-22\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-23\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-24\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-25\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-26\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-27\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-28\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-29\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-30\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540},{\"date\":\"2021-08-31\",\"startSec\":25200,\"endSec\":64800,\"max_minutes\":540}]";

            string[] stringSeparators = new string[] { "date", };
            string[] result;
            result = testsplit.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            //Show(result);


            for (int i = 1; i < result.Length; i++)
            {
               
                string sub = result[i].Substring(3,10);
                //Console.WriteLine(sub);
            }
            
            // =========================START FROM HERE==============================

            //create list to store data from MYSQL
            List<Jobs> jobset = new List<Jobs>();
            List<Staff> staffset = new List<Staff>();
            List<Duty> dateDuty = new List<Duty>();

            List<string> horizon = new List<string>();

            int winNum = 0; //the max windows number( for assigning jobs)

            //read jobs from MySQL
            string cs = @"Server = 127.0.0.1; Port = 3306; Database = data1 ; Uid = root; Pwd = rex840406";
            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();

                string sql = "SELECT jobid, priority, duration, dates, windows,tags FROM schedule_job";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader.GetString(0));                            
                            int pri = int.Parse(reader.GetString(1));
                            int dur = int.Parse(reader.GetString(2))*60;
                            var dates_ = reader.GetString(3);
                            string win = reader.GetString(4);
                            var tag = reader.GetString(5);
                            

                            //**DATES preprocess 
                                //trim and split dates data
                            string Trimmed = dates_.Trim('[',']'); //trim the start and end 
                            string[] splitdate = Trimmed.Split(','); //split

                            List<string> jobdate_ = new List<string>();
                            foreach (string date in splitdate)
                            { 
                                string trim = date.Trim('"');
                                jobdate_.Add(trim);
                                //Console.Write("this {0}", trim);
                            }
                            string[] jobdate = jobdate_.ToArray();

                            //**TAGS preprocess 
                            string trimtag = tag.Trim('[', ']'); //trim the start and end 
                            string[] splittag = trimtag.Split(','); //split
                            int[] jobtag = Array.ConvertAll(splittag, s => int.Parse(s)); //convert string[] to int[]

                            //**WINDOWS preprocess
                            
                            List<int> winstr_ = new List<int>();
                            List<int> winend_ = new List<int>();
                            //Split on one or more non-digit characters.
                            //The Regex.Split method will return the numbers in string form. The result array may also contain empty strings.
                            //The pattern "\D+" indicates that we want to use any number of one or more non-digit characters as a delimiter.
                            string[] number = Regex.Split(win, @"\D+");
                            int k = 0;
                            foreach (string value in number)
                            {
                                if (!string.IsNullOrEmpty(value))
                                { 
                                    ++k; // to tell odd or even
                                    int i = int.Parse(value);
                                    if (k % 2 != 0)
                                        winstr_.Add(i);                                    
                                    else
                                        winend_.Add(i);                                    
                                }
                            }
                            
                            //print windows
                            foreach (int i in winstr_)
                            {
                                //Console.WriteLine("winstr: {0}", i);
                            }
                            foreach (int i in winend_)
                            {
                                //Console.WriteLine("winend: {0}", i);
                            }

                            int[] winstr = winstr_.ToArray();
                            int[] winend = winend_.ToArray();

                            //Record the max window number that any job has
                            if (winstr_.Count() > winNum)
                                winNum = winstr_.Count();


                            //Print data fetched
                            //Console.WriteLine("id = {0}, priority={1}, duration={2},", id, pri, dur);

                            //**JOBSET add data to jobset
                            jobset.Add(new Jobs() { jobID = id, priority = pri, duration = dur,
                                job_dates = jobdate, win_str= winstr, win_end=winend, tags = jobtag  });
                            
                        

                            //**TIME HORIZON, create time horizon set 
                            foreach ( string date in splitdate)
                            {
                                string trim = date.Trim('"');
                                if (horizon.Count() == 0) //add the first one
                                    horizon.Add(trim);
                                else if (!horizon.Contains(trim)) //only add different dates
                                    horizon.Add(trim);

                            }
                            
                                

                        }                    
                    }                
                }            
            }


            //print windows in jobset
            for (int i = 0; i < jobset.Count(); i++)
            {
                foreach (int j in jobset[i].win_str)
                {
                    //Console.WriteLine("job{0} winstr: {1}", jobset[i].jobID, j);
                }
            }

            horizon.Sort(); // sort by dates
            // print time horizon
            //Console.WriteLine();
            foreach (string time in horizon)
            { 
                //Console.WriteLine("{0} ", time);
            }
                



            //read Staff from MySQL
            string cs1 = @"Server = 127.0.0.1; Port = 3306; Database = data1 ; Uid = root; Pwd = rex840406";
            using (var conn = new MySqlConnection(cs1))
            {
                conn.Open();

                string sql = "SELECT personid, tags, dates, max_miles FROM schedule_operative";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = int.Parse(reader.GetString(0));
                            var tag = reader.GetString(1);
                            string dates_ = reader.GetString(2);
                            int mile = int.Parse(reader.GetString(3));
                            
                                                       
                            //**TAGS preprocess 
                            string trimtag = tag.Trim('[', ']'); //trim the start and end 
                            string[] splittag = trimtag.Split(','); //split
                            int[] stafftag = Array.ConvertAll(splittag, s => int.Parse(s)); //convert string[] to int[]


                            //**WORKING WINDOWS preprocess
                            //create list
                            List<string> workdate_ = new List<string>();
                            List<int> workstr_ = new List<int>();
                            List<int> workend_ = new List<int>();
                            List<int> max_minutes_ = new List<int>();

                            //split by date 
                            string[] stringSeparator = new string[] { "date", };                            
                            string[] bydate = dates_.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                            
                            // working dates
                            for (int i = 1; i < bydate.Length; i++) //from second substring because the first splited string is [{"
                            {
                                string sub = bydate[i].Substring(3, 10); //get substring, Substring(start index, length)
                                workdate_.Add(sub);                                
                            }

                            //working dates start time
                            for (int i = 1; i < bydate.Length; i++)
                            {
                                int k = bydate[i].IndexOf("startSec");
                                int sub = int.Parse(bydate[i].Substring(k+10, 5));
                                workstr_.Add(sub);
                                //Console.WriteLine(sub);
                            }

                            //working dates end time
                            for (int i = 1; i < bydate.Length; i++)
                            {
                                int k = bydate[i].IndexOf("endSec");
                                int sub = int.Parse(bydate[i].Substring(k+8, 5));
                                workend_.Add(sub);
                                //Console.WriteLine(sub);
                            }
                            
                            //working date max working minutes
                            for (int i = 1; i < bydate.Length; i++)
                            {
                                int k = bydate[i].IndexOf("max_minutes");
                                string sub_ = bydate[i].Substring(k + 13);
                                int sub = int.Parse(sub_.TrimEnd('{', '}', '[', ']', '\"',','));
                                max_minutes_.Add(sub);
                                //Console.WriteLine(sub);
                            }



                            // tranform list to array
                            string[] workdate = workdate_.ToArray();
                            int[] workstr = workstr_.ToArray();
                            int[] workend = workend_.ToArray();
                            int[] workmax = max_minutes_.ToArray();                            


                            //**staff SET, add data to staff set
                            staffset.Add(new Staff() { personID = id, tags = stafftag, work_dates = workdate, work_str = workstr, work_end = workend, max_miles = mile, max_minutes = workmax });


                        }
                    }
                }
            }

            //print working start in staffset
            for (int i = 0; i < staffset.Count(); i++)
            {
                for (int j = 0; j < staffset[i].work_dates.Count(); j++)
                {
                    //Console.WriteLine("staff {0} on {1} from {2} to {3}, max:{4}", staffset[i].personID, staffset[i].work_dates[j], staffset[i].work_str[j], staffset[i].work_end[j],staffset[i].max_minutes[j]);

                }
            }

            //add duty by dates
            foreach (string DATE in horizon)
            {
                List<Jobs> dutybydate = new List<Jobs>();
                for (int i = 0; i < jobset.Count(); i++)
                {
                    foreach (string date in jobset[i].job_dates)
                    {
                        if (date.Contains(DATE)) // if job in jobset contains the specific available date
                        {                            
                            dutybydate.Add(new Jobs() //then collect those jobs together 
                            {
                                jobID = jobset[i].jobID,
                                priority = jobset[i].priority,
                                duration = jobset[i].duration,
                                job_dates = jobset[i].job_dates,
                                win_str = jobset[i].win_str,
                                win_end = jobset[i].win_end,
                                tags = jobset[i].tags
                            });
                        }                   
                    }
                }
                //add dates to duty list
                dateDuty.Add(new Duty() { date = DATE, dutyset =  dutybydate  });

            }

            /*
            //print data in duty list
            for (int i = 0; i < dateDuty.Count(); i++)
            {
                Console.WriteLine(); 
                Console.WriteLine(dateDuty[i].date);
                for (int j = 0; j < dateDuty[i].dutyset.Count(); j++)
                {
                    Console.Write("{0} ", dateDuty[i].dutyset[j].jobID);
                }
            }
            */


            //==========================Find feasible jobs for specific dates===================================
            for (int d = 0; d < horizon.Count(); d++)
            {
                //store the possible job for each staff
                List<Jobs>[] capable_ = new List<Jobs>[staffset.Count()];

                //store the assignment for each staff
                List<assign>[] initial = new List<assign>[staffset.Count()];

                //store the available windows for each staff
                List<available>[] available_wins = new List<available>[staffset.Count()];

                //Dayoff assignment
                int[] none = {0};
                string[] nonedate = {"none"};
                List<Jobs> day_off = new List<Jobs>();
                day_off.Add(new Jobs()
                {
                    jobID = 0,
                    priority = 0,
                    duration = 0,
                    job_dates = nonedate,
                    win_str = none,
                    win_end = none,
                    tags = none,
                });
                //Jobs day_off = new Jobs ( 0, 0, 0, nonedate, none, none, none);

                //STEP 1: Create possible job list for each staff on each date

                // 1 Tag Check:  Match jobs with people ------------------------------------------------------------
                for (int i = 0; i < staffset.Count(); i++)
                {
                    // create each staff's capable list
                    capable_[i] = new List<Jobs>();

                    // create each staff's assign list
                    initial[i] = new List<assign>();

                    // create each staff's available window list
                    available_wins[i] = new List<available>();

                    for (int j = 0; j < dateDuty[d].dutyset.Count(); j++)
                    {
                        //***Date check: check if staff works on that date
                        string dateCheck = horizon[d];
                        int winCheck = 0;

                        //2.1 check staff work on this date
                        bool b = staffset[i].work_dates.Contains(horizon[d]);
                        if (b)
                        {
                            //2.2 check staff max working minutes on that date is not 0
                            int index = Array.IndexOf(staffset[i].work_dates, horizon[d]);
                            if (staffset[i].max_minutes[index] > 0)
                            {
                                //2.3 Windows check
                                //Every job has single or multiple windows
                                for (int k = 0; k < dateDuty[d].dutyset[j].win_str.Length; k++)
                                {

                                    int staffstart = staffset[i].work_str[index];
                                    int staffend = staffset[i].work_end[index];
                                    int jobstart = dateDuty[d].dutyset[j].win_str[k];
                                    int jobend = dateDuty[d].dutyset[j].win_end[k];

                                    //Count the staff and job windows overlap  
                                    int overlap_lw = Math.Max(staffstart, jobstart);
                                    int overlap_up = Math.Min(staffend, jobend);
                                    int overlap = overlap_up - overlap_lw;

                                    // overlap > 0 means two windows have overlap
                                    // overlap > duration means job can be done within the overlap 
                                    if (overlap > 0 && overlap >= dateDuty[d].dutyset[j].duration)
                                    {
                                        winCheck += 1;
                                        //Console.WriteLine("staff {0} job {1} overlap = {2}", staffset[i].personID, dateDuty[d].dutyset[j].jobID, overlap);
                                        //Console.WriteLine("Add");
                                    }
                                    else
                                    {
                                        //Console.WriteLine("staff {0} job {1} overlap = {2}", staffset[i].personID, dateDuty[d].dutyset[j].jobID, overlap);
                                        //Console.WriteLine("NotAdd");
                                    }
                                }

                                if (winCheck > 0)
                                {

                                    bool except = dateDuty[d].dutyset[j].tags.Except(staffset[i].tags).Any();  //compare two tags array
                                    if (!except) //False means a staff has all the tags a job needs
                                    {

                                        capable_[i].Add(new Jobs()
                                        {
                                            jobID = dateDuty[d].dutyset[j].jobID,
                                            priority = dateDuty[d].dutyset[j].priority,
                                            duration = dateDuty[d].dutyset[j].duration,
                                            job_dates = dateDuty[d].dutyset[j].job_dates,
                                            win_str = dateDuty[d].dutyset[j].win_str,
                                            win_end = dateDuty[d].dutyset[j].win_end,
                                            tags = dateDuty[d].dutyset[j].tags
                                        });
                                    }

                                }



                            }
                            //else
                            //Console.WriteLine("staff {0} max working time on {1} is 0", staffset[i].personID, horizon[d]);

                        }
                        //else
                        //Console.WriteLine("staff {0} doesnt work on {1}", staffset[i].personID, horizon[d]);

                    }


                }
                //print the match result


                Console.WriteLine("\n\n{0}", horizon[d]);
                for (int i = 0; i < staffset.Count(); i++)
                {
                    Console.Write("staff {0} can do: ", staffset[i].personID);
                    for (int j = 0; j < capable_[i].Count(); j++)
                        Console.Write(" {0}", capable_[i][j].jobID);
                    Console.WriteLine();
                }
                Console.WriteLine("\n");

                //Step 2: assign

                Console.WriteLine("Max winodw number is {0}", winNum);


                for (int i = 0; i < staffset.Count(); i++)
                {

                    //================== Initial Assignment =============================================
                    
                    //find the staff work start and end on a specific date
                    //index == -1 when the system can't find index of date
                    int index = Array.IndexOf(staffset[i].work_dates, horizon[d]);
                    //Console.WriteLine("index={0}\n", index);
                    //Console.WriteLine("staff {0} \n", staffset[i].personID);

                    if (index < 0)
                    {
                        initial[i].Add(new assign
                        {
                            job = day_off[0],
                            jobID = 0,
                            job_str = 0,
                            job_end = 0,
                            totalwork = 0,
                        });
                        Console.WriteLine("\nstaff {0} doesn't work on the date", staffset[i].personID);
                    }
                    else 
                    {
                        if (capable_[i].Count() == 0)
                            Console.WriteLine("\nstaff {0} works, but has no capable job on the date", staffset[i].personID);

                        else if(capable_[i].Count() > 0)
                        {
                            int temp = 0;
                            while (capable_[i].Count() > 0)
                            {
                            //Part1: Update available Windows  ======================================
                                int staff_str;
                                int staff_end;

                                available_wins[i].Clear();

                                //Can't find date in staffset workdate, index == -1
                                if (index < 0)
                                {
                                    available_wins[i].Add(new available
                                    {
                                        str = 0,
                                        end = 0,
                                        duration = 0,
                                    });
                                    //Console.WriteLine("\nstaff {0} doesn't work on the date", staffset[i].personID);

                                }

                                //staff work on that day and not assigned any job
                                else if (index >= 0 && initial[i].Count() == 0)
                                {
                                    staff_str = staffset[i].work_str[index];
                                    staff_end = staffset[i].work_end[index];
                                    available_wins[i].Add(new available
                                    {
                                        str = staff_str,
                                        end = staff_end,
                                        duration = staff_end - staff_str,
                                    });

                                    //Console.WriteLine("\nstaff {0} available from {1} to {2}, no duty ", staffset[i].personID, staff_str, staff_end);

                                }

                                //staff work on that day and have duty
                                else if (index >= 0 && initial[i].Count() > 0)
                                {
                                    //Console.WriteLine("\nstaff {0} works and have duty ", staffset[i].personID);

                                    staff_str = staffset[i].work_str[index];
                                    staff_end = staffset[i].work_end[index];
                                    //Console.WriteLine("\nstaffstr: {0}", staff_str);
                                    //for (int n = 0; n < initial[i].Count();n++)
                                    //Console.WriteLine("job {0} str:{1} end:{2} dur: {3}", initial[i][n].jobID, initial[i][n].job_str, initial[i][n].job_end, initial[i][n].job.duration );

                                    int dutyNum = initial[i].Count();

                                    //for the first job
                                    if (initial[i][0].job_str > staff_str)
                                    {
                                        available_wins[i].Add(new available
                                        {
                                            str = staff_str,
                                            end = initial[i][0].job_str,
                                            duration = initial[i][0].job_str - staff_str,
                                        });

                                        //Console.WriteLine("staff {0} available from {1} to {2}", staffset[i].personID, staff_str, initial[i][0].job_str);
                                    }

                                    //for the middle
                                    if (dutyNum >= 2)
                                    {
                                        for (int k = 1; k < dutyNum; k++)
                                        {
                                            if (initial[i][k].job_str > initial[i][k - 1].job_end)
                                            {
                                                available_wins[i].Add(new available
                                                {
                                                    str = initial[i][k - 1].job_end,
                                                    end = initial[i][k].job_str,
                                                    duration = initial[i][k].job_str - initial[i][k - 1].job_end,
                                                });
                                            
                                                //Console.WriteLine("staff {0} available from {1} to {2}", staffset[i].personID, initial[i][k - 1].job_end, initial[i][k].job_str);
                                            }
                                        }

                                    }

                                    // for the last job
                                    if (initial[i][dutyNum - 1].job_end < staff_end)
                                    {
                                        available_wins[i].Add(new available
                                        {
                                            str = initial[i][dutyNum - 1].job_end,
                                            end = staff_end,
                                            duration = staff_end - initial[i][dutyNum - 1].job_end,
                                        });

                                        //Console.WriteLine("staff {0} available from {1} to {2}", staffset[i].personID, initial[i][dutyNum - 1].job_end, staff_end);

                                    }

                                }
                                //Sort available windows 
                                available_wins[i] = available_wins[i].OrderBy(o => o.str).ToList();
                            //============== End of Update available ======================

                            //Part2: Assign the first duty  ======================================

                                if (initial[i].Count == 0) //first duty
                                {                                                 
                                    // randomly assign a job from possible job list
                                    if (capable_[i].Count() > 0) //staff has possible work 
                                    {
                                        // random generate the job index                                      
                                        Random rand = new Random(); //random object
                                        int rnd = rand.Next(capable_[i].Count());

                                        //two cases compare the start time of staffwork or possible job, and add with different job start
                                        int startMax = Math.Max(capable_[i][rnd].win_str[0], staffset[i].work_str[index]);

                                        initial[i].Add(new assign
                                        {
                                            job = capable_[i][rnd],
                                            jobID = capable_[i][rnd].jobID,
                                            job_str = startMax,
                                            job_end = (startMax + capable_[i][rnd].duration),
                                            totalwork = capable_[i][rnd].duration,
                                        });

                                        Console.WriteLine("staff {0} assign {1}, str:{2}, end:{3}", staffset[i].personID, capable_[i][rnd].jobID, startMax, startMax + capable_[i][rnd].duration);
                                        //remove the assigned job from the staff capable list
                                        capable_[i].RemoveAt(rnd);

                                    }                                   
                                }
                            
                            //Part3: Assign the rest of jobs  ======================================    
                                else if(initial[i].Count > 0) //after first duty, to assign duty based on available list
                                {
                                    if (capable_[i].Count() > 0) //staff has possible work 
                                    {

                                        // random generate the job index                                      
                                        Random rand = new Random(); //random object
                                        int rnd = rand.Next(capable_[i].Count());

                                        bool assigned = false;
                                        for (int j = 0; j < available_wins[i].Count(); j++) //for every available windows
                                        {
                                            //available time must be bigger or equal to the job
                                            if (available_wins[i][j].duration >= capable_[i][rnd].duration)
                                            {
                                                //multiple windows
                                                for (int k = 0; k < capable_[i][rnd].win_str.Count(); k++)
                                                {
                                                    int overlap_lw = Math.Max(capable_[i][rnd].win_str[k], available_wins[i][j].str);
                                                    int overlap_up = Math.Min(capable_[i][rnd].win_end[k], available_wins[i][j].end);
                                                    int overlap = overlap_up - overlap_lw;
                                                    if (overlap >= capable_[i][rnd].duration)
                                                    {
                                                        initial[i].Add(new assign
                                                        {
                                                            job = capable_[i][rnd],
                                                            jobID = capable_[i][rnd].jobID,
                                                            job_str = overlap_lw,
                                                            job_end = (overlap_lw + capable_[i][rnd].duration),
                                                            totalwork = capable_[i][rnd].duration,
                                                        });
                                                        Console.WriteLine("staff {0} assign {1}, str:{2}, end:{3}", staffset[i].personID, capable_[i][rnd].jobID, overlap_lw, overlap_lw + capable_[i][rnd].duration);
                                                        capable_[i].RemoveAt(rnd);

                                                        assigned = true;
                                                        break;
                                                    }
                                                }// multiple window

                                            } //available window is long enough 

                                            if (assigned)
                                                break;

                                        }//for every available windows


                                    }//staff has possible work
                                }//after first duty to assign duty 

                            //============== End of assigning jobs ======================
                                //Sort initial assignment
                                initial[i] = initial[i].OrderBy(o => o.job_str).ToList();

                                


                                temp = temp + 1;
                                if (temp > 5)
                                    break;
                            }//do while

                                Console.WriteLine("\nstaffstr: {0}", staffset[i].work_str[index]);
                                for (int n = 0; n < initial[i].Count(); n++)
                                    Console.WriteLine("job {0} str:{1} end:{2} dur: {3}", initial[i][n].jobID, initial[i][n].job_str, initial[i][n].job_end, initial[i][n].job.duration);
                                for (int n = 0; n < available_wins[i].Count(); n++)
                                    Console.WriteLine("staff {0} available from {1} to {2}", staffset[i].personID, available_wins[i][n].str, available_wins[i][n].end);

                        }//capable > 0
                    }//index >= 0 

                    /*
                    //Case 1: staff doesn't work this date, then assign dayoff
                    if (initial[i].Count == 0) //first duty
                    {
                        //if the staff doesnt work on this date, add dayoff as the first and only duty
                        if (index < 0)
                        {
                            initial[i].Add(new assign
                            {
                                job = day_off[0],
                                jobID = 0,
                                job_str = 0,
                                job_end = 0,
                                totalwork = 0,
                            });
                        }
                        //Case 2: staff works on this date, randomly assign a job from possible job list
                        else
                        {
                            if (capable_[i].Count() > 0) //staff has possible work 
                            {
                                // random generate the job index                                      
                                Random rand = new Random(); //random object
                                int rnd = rand.Next(capable_[i].Count());

                                //two cases compare the start time of staffwork or possible job, and add with different job start
                                int startMax = Math.Max(capable_[i][rnd].win_str[0], staffset[i].work_str[index]);

                                initial[i].Add(new assign
                                {
                                    job = capable_[i][rnd],
                                    jobID = capable_[i][rnd].jobID,
                                    job_str = startMax,
                                    job_end = (startMax + capable_[i][rnd].duration),
                                    totalwork = capable_[i][rnd].duration,
                                });

                            }
                        }
                    }

                    //Sort initial assignment
                    initial[i] = initial[i].OrderBy(o => o.job_str).ToList();

                    //=================Calculate available Windows======================================

                    int staff_str;
                    int staff_end;

                    //Can't find date in staffset workdate, index == -1
                    if (index < 0) 
                    {
                        available_wins[i].Add(new available
                        {
                            str = 0,
                            end = 0,
                            duration = 0,
                        });
                        Console.WriteLine("\nstaff {0} doesn't work on the date", staffset[i].personID);                        
                        
                    }

                    //staff work on that day and not assigned any job
                    else if (index >= 0 && initial[i].Count() == 0) 
                    {
                        staff_str = staffset[i].work_str[index];
                        staff_end = staffset[i].work_end[index];
                        available_wins[i].Add(new available
                        {
                            str = staff_str,
                            end = staff_end,
                            duration = staff_end - staff_str,
                        });

                        Console.WriteLine("\nstaff {0} available from {1} to {2}, no duty ", staffset[i].personID, staff_str,staff_end);
                        
                    }
                    
                    //staff work on that day and have duty
                    else if (index >= 0 && initial[i].Count() > 0) 
                    {
                        Console.WriteLine("\nstaff {0} works and have duty ", staffset[i].personID );
                        
                        staff_str = staffset[i].work_str[index];
                        staff_end = staffset[i].work_end[index];
                        Console.WriteLine("jobstr = {0}, duration ={1} staffstr = {2}", initial[i][0].job_str , initial[i][0].job.duration, staff_str);

                        int dutyNum = initial[i].Count();
                        
                        //for the first job
                        if (initial[i][0].job_str > staff_str)
                        {
                            available_wins[i].Add(new available
                            {
                                str = staff_str,
                                end = initial[i][0].job_str,
                                duration = initial[i][0].job_str - staff_str,
                            });

                            Console.WriteLine("staff {0} available from {1} to {2}", staffset[i].personID, staff_str, initial[i][0].job_str);
                        }

                        //for the middle
                        if (dutyNum >= 2)
                        {
                            for (int k = 1; k < dutyNum; k++)
                            {
                                available_wins[i].Add(new available
                                {
                                    str = initial[i][k-1].job_end,
                                    end = initial[i][k].job_str,
                                    duration = initial[i][k].job_str - initial[i][k - 1].job_end,
                                });

                                Console.WriteLine("staff {0} available from {1} to {2}", staffset[i].personID, initial[i][dutyNum - 1].job_end, staff_end);
                                
                            }
                        
                        }

                        // for the last job
                        if (initial[i][dutyNum - 1].job_end < staff_end)
                        {
                            available_wins[i].Add(new available
                            {
                                str = initial[i][dutyNum - 1].job_end,
                                end = staff_end,
                                duration = staff_end - initial[i][dutyNum - 1].job_end,
                            });

                            Console.WriteLine("staff {0} available from {1} to {2}", staffset[i].personID, initial[i][dutyNum - 1].job_end, staff_end);
                            

                        }
                        else 
                        {
                            //Not adding anything to available list if initial[i][dutyNum - 1].job_end >= staff_end
                        }

                    }
                    //Sort available windows 
                    available_wins[i] = available_wins[i].OrderBy(o => o.str).ToList();

                    */







                }//staff loop
            

                    Console.WriteLine("\n\n{0}", horizon[d]);
                    for (int i = 0; i < staffset.Count(); i++)
                    {
                        Console.Write("staff {0} assign: ", staffset[i].personID);
                        for (int j = 0; j < initial[i].Count(); j++)
                            Console.Write(" {0}", initial[i][j].jobID);
                        Console.WriteLine();
                    }
                    Console.WriteLine("\n");





                
            }//date loop




            /*

            string sql = "SELECT personid,tag, dayoff, max_minutes, max_miles FROM test.operative_test";
            using var cmd = new MySqlCommand(sql, con);
            using MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine("id{0} tag{1} dayoff{2} minute{3} mile{4}", rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(3), rdr.GetInt32(4));
              
                personid.Add(rdr.GetInt32(0));
                worker_tags.Add(rdr.GetString(1));
                dayoff.Add(rdr.GetString(2));
                max_minutes.Add(rdr.GetInt32(3));
                max_miles.Add(rdr.GetInt32(4));
            }
*/
            // job data
            List<int> jobid = new List<int>();
            List<int> priority = new List<int>();
            List<int> duration = new List<int>();
            List<int> dates = new List<int>();
            List<int> window_start = new List<int>();
            List<int> window_end = new List<int>();
            List<int> job_tags = new List<int>();

            //read operatives
            //string cs1 = @"Server = 127.0.0.1; Port = 3306; Database = test; Uid = root; Pwd = rex840406";
            /*          using var con1 = new MySqlConnection(cs);
                      con1.Open();
                      string sql1 = "SELECT jobid,priority,duration,dates, windows,tag FROM test.schedule_job";
                      using var cmd1 = new MySqlCommand(sql1, con1);
                      using MySqlDataReader rdr1 = cmd1.ExecuteReader();

                      int k = 1;
                      while (rdr1.Read() && k < 50) 
                      {
                          //Console.WriteLine("{0} Tags: {1}",k, rdr1.GetString(5));
                          k += 1;
                          Console.WriteLine("The result is {0}", rdr1.GetString(4));
                      }

                      for (int i = 0; i <50 ; i++)
                      {

                      }


                      worker_tags.ForEach(PrintStr);
                      dayoff.ForEach(PrintStr);
            */

            //TODO
            //max_minute
            //dayoff
            //jobs not overlay
            List<location> drive = new List<location>();

            //Dictionary<location, int> driving = new Dictionary<location, int>();

            drive.Add(new location() { from = 1, to = 8, duration = 1800 });
            drive.Add(new location() { from = 1, to = 2, duration = 1200 });
            drive.Add(new location() { from = 1, to = 9, duration = 1200 });
            drive.Add(new location() { from = 8, to = 13, duration = 1800 });
            drive.Add(new location() { from = 2, to = 9, duration = 1200 });
            drive.Add(new location() { from = 9, to = 14, duration = 1200 });
            drive.Add(new location() { from = 9, to = 2, duration = 1200 });
            drive.Add(new location() { from = 2, to = 14, duration = 1200 });
            drive.Add(new location() { from = 4, to = 10, duration = 1600 });
            drive.Add(new location() { from = 10, to = 15, duration = 1600 });
            drive.Add(new location() { from = 5, to = 11, duration = 1800 });
            drive.Add(new location() { from = 11, to = 16, duration = 1800 });
            drive.Add(new location() { from = 6, to = 12, duration = 1200 });
            drive.Add(new location() { from = 12, to = 17, duration = 1200 });
            
            drive.Add(new location() { from = 101, to = 1, duration = 5000 });
            drive.Add(new location() { from = 101, to = 2, duration = 1200 });
            drive.Add(new location() { from = 101, to = 3, duration = 1200 });
            drive.Add(new location() { from = 101, to = 4, duration = 1200 });
            drive.Add(new location() { from = 101, to = 5, duration = 1200 });
            drive.Add(new location() { from = 101, to = 6, duration = 1200 });
            drive.Add(new location() { from = 101, to = 7, duration = 1200 });
            drive.Add(new location() { from = 101, to = 8, duration = 1225 });
            drive.Add(new location() { from = 101, to = 9, duration = 1200 });
            drive.Add(new location() { from = 101, to = 10, duration = 1200 });
            drive.Add(new location() { from = 101, to = 11, duration = 1200 });
            drive.Add(new location() { from = 101, to = 12, duration = 1200 });
            drive.Add(new location() { from = 101, to = 13, duration = 1278 });
            drive.Add(new location() { from = 101, to = 14, duration = 1200 });
            drive.Add(new location() { from = 101, to = 15, duration = 1200 });
            drive.Add(new location() { from = 101, to = 16, duration = 1200 });
            drive.Add(new location() { from = 101, to = 17, duration = 1200 });

            drive.Add(new location() { from = 102, to = 1, duration = 1300 });
            drive.Add(new location() { from = 102, to = 2, duration = 1300 });
            drive.Add(new location() { from = 102, to = 3, duration = 1300 });
            drive.Add(new location() { from = 102, to = 4, duration = 1300 });
            drive.Add(new location() { from = 102, to = 5, duration = 1300 });
            drive.Add(new location() { from = 102, to = 6, duration = 1300 });
            drive.Add(new location() { from = 102, to = 7, duration = 1300 });
            drive.Add(new location() { from = 102, to = 8, duration = 1300 });
            drive.Add(new location() { from = 102, to = 9, duration = 1300 });
            drive.Add(new location() { from = 102, to = 10, duration = 1300 });
            drive.Add(new location() { from = 102, to = 11, duration = 1300 });
            drive.Add(new location() { from = 102, to = 12, duration = 1300 });
            drive.Add(new location() { from = 102, to = 13, duration = 1300 });
            drive.Add(new location() { from = 102, to = 14, duration = 1300 });
            drive.Add(new location() { from = 102, to = 15, duration = 1300 });
            drive.Add(new location() { from = 102, to = 16, duration = 1300 });
            drive.Add(new location() { from = 102, to = 17, duration = 1300 });

            drive.Add(new location() { from = 103, to = 1, duration = 1300 });
            drive.Add(new location() { from = 103, to = 2, duration = 1300 });
            drive.Add(new location() { from = 103, to = 3, duration = 1300 });
            drive.Add(new location() { from = 103, to = 4, duration = 1300 });
            drive.Add(new location() { from = 103, to = 5, duration = 1300 });
            drive.Add(new location() { from = 103, to = 6, duration = 1300 });
            drive.Add(new location() { from = 103, to = 7, duration = 1300 });
            drive.Add(new location() { from = 103, to = 8, duration = 1300 });
            drive.Add(new location() { from = 103, to = 9, duration = 1300 });
            drive.Add(new location() { from = 103, to = 10, duration = 1300 });
            drive.Add(new location() { from = 103, to = 11, duration = 1300 });
            drive.Add(new location() { from = 103, to = 12, duration = 1300 });
            drive.Add(new location() { from = 103, to = 13, duration = 1300 });
            drive.Add(new location() { from = 103, to = 14, duration = 1300 });
            drive.Add(new location() { from = 103, to = 15, duration = 1300 });
            drive.Add(new location() { from = 103, to = 16, duration = 1300 });
            drive.Add(new location() { from = 103, to = 17, duration = 1300 });
            // drive.Add(new location() { from = 3, to = 1, duration = 1600 });
            //drive.Add(new location() { from = 3, to = 2, duration = 1600 });



            //work data
            List<Job> jobs = new List<Job>();
            jobs.Add(new Job() { jobID = 1, job_dates = 1, win_start = 36000, win_end = 43200, tags = new int[] { 14  }, duration = 7200});
            jobs.Add(new Job() { jobID = 2, job_dates = 1, win_start = 36000, win_end = 43200, tags = new int[] { 13 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 3, job_dates = 2, win_start = 36000, win_end = 43200, tags = new int[] { 11 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 4, job_dates = 2, win_start = 36000, win_end = 43200, tags = new int[] { 12 }, duration = 5400 });
            jobs.Add(new Job() { jobID = 5, job_dates = 2, win_start = 36000, win_end = 43200, tags = new int[] { 13 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 6, job_dates = 3, win_start = 36000, win_end = 43200, tags = new int[] { 11 }, duration = 3600 });
            jobs.Add(new Job() { jobID = 7, job_dates = 3, win_start = 36000, win_end = 43200, tags = new int[] { 12 }, duration = 7200 });

            jobs.Add(new Job() { jobID = 8,  job_dates = 1, win_start = 46800, win_end = 54000, tags = new int[] { 11 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 9,  job_dates = 1, win_start = 46800, win_end = 54000, tags = new int[] { 13 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 10, job_dates = 2, win_start = 46800, win_end = 54000, tags = new int[] { 12 }, duration = 5400 });
            jobs.Add(new Job() { jobID = 11, job_dates = 2, win_start = 46800, win_end = 54000, tags = new int[] { 13 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 12, job_dates = 3, win_start = 46800, win_end = 54000, tags = new int[] { 11 }, duration = 3600 });

            jobs.Add(new Job() { jobID = 13, job_dates = 1, win_start = 57600, win_end = 64800, tags = new int[] { 11 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 14, job_dates = 1, win_start = 57600, win_end = 64800, tags = new int[] { 13 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 15, job_dates = 2, win_start = 57600, win_end = 64800, tags = new int[] { 12 }, duration = 5400 });
            jobs.Add(new Job() { jobID = 16, job_dates = 2, win_start = 57600, win_end = 64800, tags = new int[] { 13 }, duration = 7200 });
            jobs.Add(new Job() { jobID = 17, job_dates = 3, win_start = 57600, win_end = 64800, tags = new int[] { 11 }, duration = 5400 });

            //driving duty will be added here
            List<Job> driving = new List<Job>();

            List<Job> breaktime = new List<Job>();
            breaktime.Add(new Job() { duration = 3600 }); //each operator has one hour break per day

            List<Job> buffer = new List<Job>();
            buffer.Add(new Job() { duration = 600 }); //10 mins buffer in front of every work









            // special dayoff job
            List<Job> dayoffjobs = new List<Job>();
            dayoffjobs.Add(new Job() { jobID = 0, job_dates = 1, win_start = 1, win_end = 86399, tags = new int[] {  } , priority = 1 }); 
            dayoffjobs.Add(new Job() { jobID = 0, job_dates = 2, win_start = 1, win_end = 86399, tags = new int[] {  } , priority = 2 });
            dayoffjobs.Add(new Job() { jobID = 0, job_dates = 3, win_start = 1, win_end = 86399, tags = new int[] {  } , priority = 3 });

            //person data
            List<Person> person = new List<Person>();
            person.Add(new Person() { personID = 1, p_tags = new int[] { 11,14 }, dayoff = new int[] {   } , init_pos = 101 });
            person.Add(new Person() { personID = 2, p_tags = new int[] { 12 }, dayoff = new int[] { 1 } , init_pos = 102 });
            person.Add(new Person() { personID = 3, p_tags = new int[] { 13,14 }, dayoff = new int[] { 3 } , init_pos = 103 });

            //two ways to add duty component to dayschedule class
            List<DaySchedule> test = new List<DaySchedule>();
            test.Add(new DaySchedule() { personID = 1, duty = new List<Job>() , dutyTime = +10});
            
            test[0].duty.Add(jobs[1]);
            test[0].dutyTime += 20;

            int ji = test[0].duty.Count();
            test.Add(new DaySchedule() { personID = 1, duty = new List<Job>() {jobs[0], jobs[1]} });

            Console.WriteLine("here is duty {0}", test[0].duty[0].win_start);
            Console.WriteLine("here is time {0}", test[0].dutyTime);

            //dayoffjobs.Add(jobs[0]); 
            //Console.WriteLine("here is test priority {0}", dayoffjobs[3].priority);

            foreach (int x in dayoffjobs[0].tags)
            {
                Console.WriteLine("here is {0}", x);
            }

            //store the possible job for each person
            List<Job>[] capable = new List<Job>[person.Count()];

            //****Tag Constraint: Match jobs with people ------------------------------------------------------------
            for (int i = 0; i < person.Count(); i++)
            {
                capable[i] = new List<Job>(); // create each person's capable list
                for (int j = 0; j < jobs.Count(); j++)
                {
                    bool except = jobs[j].tags.Except(person[i].p_tags).Any();  //compare two tags array
                    if (!except) //False means a person has all the tags a job needs
                    {
                        capable[i].Add(new Job() { jobID = jobs[j].jobID, job_dates = jobs[j].job_dates, win_start = jobs[j].win_start, win_end = jobs[j].win_end, tags = jobs[j].tags, duration = jobs[j].duration });
                        //capable[i].Add(jobs[j]); //add possible job to that person
                    }
                }
            }
            //print the match result
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person {0} can do:", i + 1);
                for (int j = 0; j < capable[i].Count(); j++)
                    Console.Write(" {0}", capable[i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");



            //--------d1----Initial solution ----------------------------------------------
           


            //store the possible job for each person
            List<Job>[] day1 = new List<Job>[person.Count()];
            List<location>[] temp_dri = new List<location>[person.Count()];
            List<Job>[] sol_day1 = new List<Job>[person.Count()];
            List<DaySchedule>[] solday1 = new List<DaySchedule>[person.Count()];
            List<Job>[] day1_order = new List<Job>[person.Count()]; // to find the first add duty

            /*List<Job>[][] day = new List<Job>[person.Count()][];

            
            for (int i = 0; i < person.Count(); i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    day[j][i] = new List<Job>();
                    day[j][i].Add(jobs[0]);
                    //day[j][i] = capable[i].FindAll(x => x.job_dates == j);
                }
            }
            //print possible duty
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person{0} possible job in day1:", i + 1);
                for (int j = 0; j < day[0][i].Count(); j++)
                    Console.Write(" {0}", day[0][i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");
            */

            //***** Winodw Constraints: find all date 1 job for all people and sort it by windows start time---------------



            for (int i = 0; i < person.Count(); i++)
            {
                day1[i] = new List<Job>();
                temp_dri[i] = new List<location>();
                sol_day1[i] = new List<Job>();
                solday1[i] = new List<DaySchedule>();
                //store day1's possible duty with driving time
                //day1_order[i] = new List<Job>();


                //find all day1 job
                day1[i] = capable[i].FindAll(x => x.job_dates == 1);
                //day1_order[i] = capable[i].FindAll(x => x.job_dates == 1);


                //***** Dayoff Constraints:---------------------------------------------------------------------------------------  
                if (person[i].dayoff.Contains(1)) //if person dayoff on day 1
                {
                    day1[i].Clear();
                    day1[i].Add(dayoffjobs.Find(x => x.jobID == 0 && x.job_dates == 1)); // Assign special dayoff job to the person

                    //day1_order[i].Clear();
                    //day1_order[i].Add(dayoffjobs.Find(x => x.jobID == 0 && x.job_dates == 1)); // Assign special dayoff job to the person
                }
            }


            
            for (int i = 0; i < person.Count(); i++)
            {
                day1_order[i] = new List<Job>(day1[i]);
                // including driving time to get job in order              
                if (day1[i][0].jobID != 0) // if it's not dayoff
                {

                    
                    for (int j = 0; j < day1[i].Count(); j++)
                    {

                        Console.WriteLine("day1 person {0} job {1} strtime {2}", i + 1, day1[i][j].jobID, day1[i][j].win_start);
    
                        //Console.WriteLine("day1_order person {0} job {1} strtime {2}", i + 1, day1_order[i][j].jobID, day1_order[i][j].win_start);

                        //day1[i][j] means the possible task j for person i on day 1
                        temp_dri[i].Add(drive.Find(x => x.from == person[i].init_pos && x.to == day1[i][j].jobID));
                        Console.WriteLine("From person {0} initial to job{1} duration = {2}", i+1, day1[i][j].jobID, temp_dri[i][j].duration);

                        //Console.WriteLine("person {0} job {1} original win_start is {2}:", i + 1, day1_order[i][j].jobID, day1_order[i][j].win_start);
                        day1[i][j].calcutate += day1[i][j].win_start + temp_dri[i][j].duration;


                        Console.WriteLine("day1 person {0} job {1} calculted starttime {2}", i + 1, day1[i][j].jobID, day1[i][j].calcutate);
                        //Console.WriteLine("===========day1_order person {0} job {1} strtime {2}", i + 1, day1_order[i][j].jobID, day1_order[i][j].win_start);

                        //Console.WriteLine("person {0} job {1} new win_start is {2}:", i + 1, day1_order[i][j].jobID, day1_order[i][j].win_start);
                        Console.WriteLine("\n");


                            
                           
                    }
                    }
                // sort day1 by window time
                day1_order[i] = day1_order[i].OrderBy(o => o.win_start).ToList();
                day1[i] = day1[i].OrderBy(o => o.calcutate).ToList();



            }
            for (int i = 0; i < person.Count(); i++)
            {
                //Console.WriteLine("day1_order for person {0} is = {1}", i + 1, day1_order[i].Count());
                Console.WriteLine("day1 for person {0} is = {1}", i + 1, day1[i].Count());
            }
                

            //print possible duty
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("No dri: person{0} possible job in day1:", i + 1);
                for (int j = 0; j < day1[i].Count(); j++)
                    Console.Write(" {0}", day1[i][j].jobID);
                    Console.WriteLine();
                Console.WriteLine("Dri: person{0} possible job in day1:", i + 1);
                for (int j = 0; j < day1[i].Count(); j++)
                    Console.Write(" {0}", day1_order[i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");


            for (int i = 0; i < day1.Count(); i++)
                for (int j = 0; j < day1[i].Count(); j++)
                    day1[i][j].calcutate = 0; 


            //solution
            for (int i = 0; i < person.Count(); i++)
            {
                //***** Only One person for a job Constraint:------------------------------------------------------------------
                // from person 2, remove all jobs in the possible duty that already done by previous people


                if (i > 0 && day1[i].Any() && day1[i][0].jobID != 0) //day[i] is not empty and the only obj is not dayoff   
                {
                    for (int k = 0; k < i; k++)
                    {
                        Console.Write("\nbefore p{0} can do ", i + 1);
                        for (int p = 0; p < day1[i].Count(); p++)
                            Console.Write(" {0}",day1[i][p].jobID);
                        
                        day1[i] = day1[i].Except(sol_day1[k]).ToList(); //Tolist turns IEnumerable(the return of except) to list  
                        //day1[i] = day1[i].Except(solday1[0].duty).ToList();

                        Console.Write("\nafter except p{0}'s duty, p{1} can do", k + 1, i + 1);
                        for (int p = 0; p < day1[i].Count(); p++)
                            Console.Write(" {0}", day1[i][p].jobID);
                    }
                }

                day1[2] = day1[2].Except(sol_day1[0]).ToList();
                for (int z = 0; z < person.Count(); z++)
                {
                    Console.WriteLine("\n=>>>>>>>>>p{0}, duty =", z + 1);
                    for (int j = 0; j < day1[z].Count(); j++)
                        Console.Write(" {0}", day1[z][j].jobID);
                }

                //***** Duration Constraints:---------------------------------------------------------------------------------           
                if (day1[i].Any()) //if day1[i] is not empty, which means there are works available for perosn i to do on this day
                {

                    /*if (!sol_day1[i].Any())
                    {
                        //the first duty is added after taking the driving time from operator's initial position
                        for (int j = 0; j < day1[i].Count(); j++)
                        {
                            List<location> temp = new List<location>();
                            // driving time from initial position to the job position
                            temp.Add(drive.Find(x => x.from == person[i].day_start && x.to == day1[i][j].jobID));
                            if (temp.Any())
                                Console.WriteLine("yes: {0}", temp[j].duration);
                            //Console.WriteLine("First job, From job{0} to job{1} duration = {2}", person[i].day_start, day1[i][j].jobID, temp[0].duration);

                            //make sure the start time of later job is behind the start time of the previous one
                            if ((work_begin + temp[0].duration) <= day1[i][j].win_start)
                            {

                                sol_day1[i].Add(day1[i][j]); //the first job in a day is always added
                                Console.WriteLine("Add first duty {0} to p{1}", day1[i][j].jobID, i + 1);
                               
                            }
                            else if (j == day1[i].Count() - 1 && !sol_day1[i].Any())
                                Console.WriteLine("There is no possible work for person i on this day.");

                        }
                    }*/
                    sol_day1[i].Add(day1[i][0]); //the first job in a day is always added

                    solday1[i].Add(new DaySchedule()
                    {
                        personID = person[i].personID,
                        p_tags = person[i].p_tags,
                        duty = new List<Job>() { day1[i][0] },
                        dutyTime = day1[i][0].duration,
                        dutyNum = 1,
                    }); 


                    //test[0].duty.Add(jobs[1]);
                    Console.WriteLine("\n====Add first duty {0} to p{1}", day1[i][0].jobID, i + 1);
                    // add duties
                    for (int j = 0; j < day1[i].Count() - 1; j++)
                    {
                        //Console.WriteLine("day{0}{1}= {2};day{3}{4}={5}", i,j, day1[i][j].win_end,i,j+1, day1[i][j+1].win_start);


                        //***** Driving Constraints:--------------------------------------------------------------------------------- 

                        //sol_day1
                        int index = sol_day1[i].Count(); // person i-1 's last duty
                        int id = sol_day1[i][index - 1].jobID;
                        int id_next = day1[i][j + 1].jobID;
                        List<location> temp_1 = new List<location>();
                        temp_1.Add(drive.Find(x => x.from == id && x.to == id_next));
                        Console.WriteLine("From job{0} to job{1} duration = {2}", id, id_next, temp_1[0].duration);
                        
                        //solday1
                        int index_1 = solday1[i][0].duty.Count(); // person i's total duty number 
                        int id_1 = solday1[i][0].duty[index_1 - 1].jobID;  // person i-1 's last duty
                        int id_next_1 = day1[i][j + 1].jobID;
                        List<location> temp_2 = new List<location>();         
                        temp_2.Add(drive.Find(x => x.from == id_1 && x.to == id_next_1));
                        Console.WriteLine("From job{0} to job{1} duration = {2}", id_1, id_next_1, temp_2[0].duration);


                        //make sure the start time of later job is behind the start time of the previous one
                        if ((sol_day1[i][index - 1].win_end + temp_1[0].duration) <= day1[i][j + 1].win_start)
                        {
                            sol_day1[i].Add(day1[i][j + 1]);
                            Console.WriteLine("=====Add {0} to p{1}", day1[i][j + 1].jobID, i + 1);
                        }
                        
                        //solday1
                        if ((solday1[i][0].duty[index_1 - 1].win_end + temp_2[0].duration) <= day1[i][j + 1].win_start)
                        {
                            solday1[i][0].duty.Add(day1[i][j + 1]);
                            Console.WriteLine("solday1 Add {0} to p{1}", day1[i][j + 1].jobID, i + 1);
                        }




                    }
                    
                }
                
            }

            Console.WriteLine("\n");
            //print solution
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person{0} duty in day1:", person[i].personID);
                for (int j = 0; j < sol_day1[i].Count(); j++)
                    Console.Write(" {0}", sol_day1[i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            //print solday1 solution
            Console.WriteLine("solday1 solution\n");
            for (int i = 0; i < person.Count(); i++)
            {                
                Console.WriteLine("person{0} duty in day1:", person[i].personID);
                for (int j = 0; j < solday1[i][0].duty.Count(); j++)
                    Console.Write(" {0}", solday1[i][0].duty[j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");



            //-------d1-----Initial solution end---------------------------------




           /*

            //-------d2-----Initial solution ----------------------------------------------



            //store the possible job for each person
            List<Job>[] day2 = new List<Job>[person.Count()];
            List<Job>[] sol_day2 = new List<Job>[person.Count()];
    

            //***** Winodw Constraints: find all date 1 job for all people and sort it by windows start time---------------
            for (int i = 0; i < person.Count(); i++)
            {
                day2[i] = new List<Job>();
                sol_day2[i] = new List<Job>();
                day2[i] = capable[i].FindAll(x => x.job_dates == 2);

                //***** Dayoff Constraints:---------------------------------------------------------------------------------------  
                if (person[i].dayoff.Contains(2)) //if person dayoff on day 1
                {
                    day2[i].Clear();
                    day2[i].Add(dayoffjobs.Find(x => x.jobID == 0 && x.job_dates == 2)); // Assign special dayoff job to the person
                }

                day2[i] = day2[i].OrderBy(o => o.win_start).ToList();
            }
            //print possible duty
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person{0} possible job in day2:", i + 1);
                for (int j = 0; j < day2[i].Count(); j++)
                    Console.Write(" {0}", day2[i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            //solution
            for (int i = 0; i < person.Count(); i++)
            {
                //***** Only One person for a job Constraint:------------------------------------------------------------------
                // from person 2, remove all jobs in the possible duty that already done by previous people

                if (i > 0 && day2[i].Any() && day2[i][0].jobID != 0) //day[i] is not empty and the only obj is not dayoff   
                {
                    for (int k = 0; k < i; k++)
                    {
                        for (int p = 0; p < day2[i].Count(); p++)
                            Console.WriteLine("before p{0} can do {1}", i + 1, day2[i][p].jobID);

                        day2[i] = day2[i].Except(sol_day2[k]).ToList(); //Tolist turns IEnumerable(the return of except) to list  

                        for (int p = 0; p < day2[i].Count(); p++)
                            Console.WriteLine("after except p{2}'s duty, p{0} can do {1}", i + 1, day2[i][p].jobID, k + 1);
                    }
                }

                //***** Duration Constraints:---------------------------------------------------------------------------------           
                if (day2[i].Any()) //if day2[i] is not empty
                {
                    sol_day2[i].Add(day2[i][0]); //the first job in a day is always added
                    Console.WriteLine("Add {0} to p{1}", day2[i][0].jobID, i + 1);
                    for (int j = 0; j < day2[i].Count() - 1; j++)
                    {
                        //Console.WriteLine("day{0}{1}= {2};day{3}{4}={5}", i,j, day2[i][j].win_end,i,j+1, day2[i][j+1].win_start);


                        //***** Driving Constraints:--------------------------------------------------------------------------------- 

                        int index = sol_day2[i].Count(); // person i-1 's last duty
                        int id = sol_day2[i][index - 1].jobID;
                        int id_next = day2[i][j + 1].jobID;
                        List<location> temp = new List<location>();
                        temp.Add(drive.Find(x => x.from == id && x.to == id_next));
                        Console.WriteLine("From job{0} to job{1} duration = {2}", id, id_next, temp[0].duration);

                        //make sure the start time of later job is behind the start time of the previous one
                        if ((sol_day2[i][index - 1].win_end + temp[0].duration) <= day2[i][j + 1].win_start)
                        {
                            sol_day2[i].Add(day2[i][j + 1]);
                            Console.WriteLine("Add {0} to p{1}", day2[i][j + 1].jobID, i + 1);
                        }
                    }
                }

            }

            Console.WriteLine("\n");
            //print solution
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person{0} duty in day2:", person[i].personID);
                for (int j = 0; j < sol_day2[i].Count(); j++)
                    Console.Write(" {0}", sol_day2[i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");



            //--------d2----Initial solution end---------------------------------

            //--------d3----Initial solution ----------------------------------------------



            //store the possible job for each person
            List<Job>[] day3 = new List<Job>[person.Count()];
            List<Job>[] sol_day3 = new List<Job>[person.Count()];
       

            //***** Winodw Constraints: find all date 1 job for all people and sort it by windows start time---------------
            for (int i = 0; i < person.Count(); i++)
            {
                day3[i] = new List<Job>();
                sol_day3[i] = new List<Job>();
                day3[i] = capable[i].FindAll(x => x.job_dates == 3);

                //***** Dayoff Constraints:---------------------------------------------------------------------------------------  
                if (person[i].dayoff.Contains(3)) //if person dayoff on day 1
                {
                    day3[i].Clear();
                    day3[i].Add(dayoffjobs.Find(x => x.jobID == 0 && x.job_dates == 3)); // Assign special dayoff job to the person
                }

                day3[i] = day3[i].OrderBy(o => o.win_start).ToList();
            }
            //print possible duty
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person{0} possible job in day3:", i + 1);
                for (int j = 0; j < day3[i].Count(); j++)
                    Console.Write(" {0}", day3[i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");

            //solution
            for (int i = 0; i < person.Count(); i++)
            {
                //***** Only One person for a job Constraint:------------------------------------------------------------------
                // from person 2, remove all jobs in the possible duty that already done by previous people

                if (i > 0 && day3[i].Any() && day3[i][0].jobID != 0) //day[i] is not empty and the only obj is not dayoff   
                {
                    for (int k = 0; k < i; k++)
                    {
                        for (int p = 0; p < day3[i].Count(); p++)
                            Console.WriteLine("before p{0} can do {1}", i + 1, day3[i][p].jobID);

                        day3[i] = day3[i].Except(sol_day3[k]).ToList(); //Tolist turns IEnumerable(the return of except) to list  

                        for (int p = 0; p < day3[i].Count(); p++)
                            Console.WriteLine("after except p{2}'s duty, p{0} can do {1}", i + 1, day3[i][p].jobID, k + 1);
                    }
                }

                //***** Duration Constraints:---------------------------------------------------------------------------------           
                if (day3[i].Any()) //if day3[i] is not empty
                {
                    sol_day3[i].Add(day3[i][0]); //the first job in a day is always added
                    Console.WriteLine("Add {0} to p{1}", day3[i][0].jobID, i + 1);
                    for (int j = 0; j < day3[i].Count() - 1; j++)
                    {
                        //Console.WriteLine("day{0}{1}= {2};day{3}{4}={5}", i,j, day3[i][j].win_end,i,j+1, day3[i][j+1].win_start);


                        //***** Driving Constraints:--------------------------------------------------------------------------------- 

                        int index = sol_day3[i].Count(); // person i-1 's last duty
                        int id = sol_day3[i][index - 1].jobID;
                        int id_next = day3[i][j + 1].jobID;
                        List<location> temp = new List<location>();
                        temp.Add(drive.Find(x => x.from == id && x.to == id_next));
                        Console.WriteLine("From job{0} to job{1} duration = {2}", id, id_next, temp[0].duration);

                        //make sure the start time of later job is behind the start time of the previous one
                        if ((sol_day3[i][index - 1].win_end + temp[0].duration) <= day3[i][j + 1].win_start)
                        {
                            sol_day3[i].Add(day3[i][j + 1]);
                            Console.WriteLine("Add {0} to p{1}", day3[i][j + 1].jobID, i + 1);
                        }
                    }
                }

            }

            Console.WriteLine("\n");
            //print solution
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person{0} duty in day3:", person[i].personID);
                for (int j = 0; j < sol_day3[i].Count(); j++)
                    Console.Write(" {0}", sol_day3[i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");



            //-------d3-----Initial solution end---------------------------------


            */

            int num_person = 20;
            Person[] operatives = new Person[num_person];
            for (int i = 0; i < num_person; i++) {        
            operatives[i] = new Person();
                {
                    operatives[i].personID = i;
                    //operatives[i].max_minutes = max_minutes[i];
                    //operatives[i].max_miles = max_miles[i];
                }
            }
            //Console.WriteLine("Operative    : Max working mins :  Max distance");
            for (int i = 0; i < num_person; i++)
            {
              //  Console.WriteLine("Operative {0}:     {1} mins     :    {2}     ", operatives[i].personID, operatives[i].max_minutes, operatives[i].max_miles);
            }





            // syntax test area
            /*
            int x = 51;
            Person[] perosns = new Person[x];


            int[] marks = new int[] { 99, 98, 92, 97, 95 };
            int[] mark = new int[] { 99, 98, 92, };
            IEnumerable<int> exp = mark.Except(marks);
            bool exp1 = marks.Except(mark).Any();
            foreach (double number in exp)
                Console.WriteLine(number);
            Console.Write("Any bool: {0}", exp1);

            bool exp2 = jobs[0].tags.Except(person[0].p_tags).Any();
            Console.Write("Any bool_2: {0}", exp2);
            */



            /*
            //int index = person.IndexOf()
            Person ok = person.Find(x => x.dayoff.Contains(1));
            Job ok1 = jobs.Find(x => x.jobID == 0 && x.job_dates == 1);


            bool cont = person[0].dayoff.Contains(1);
            Console.WriteLine("contain or not {0}", cont);

            */




            /*
             * initial solution code
             * 
            //match person with all jobs when there are only one tag for each job 
            /*
            IEnumerable<Job> p = jobs.Where(x => person.Any(z => x.tags == person[0].p_tags));
            List<Job> p1 = jobs.FindAll(x => x.tags == person[0].p_tags);
            List<Job> p2 = jobs.FindAll(x => x.tags == person[1].p_tags);
            List<Job> p3 = jobs.FindAll(x => x.tags == person[2].p_tags);

            foreach (Job obj in p1)
            {
                Console.WriteLine("p1 can do {0} ", obj.jobID);
            }
            foreach (Job obj in p2)
            {
                Console.WriteLine("p2 can do {0} ", obj.jobID);
            }
            foreach (Job obj in p3)
            {
                Console.WriteLine("p3 can do {0} ", obj.jobID);
            }
            
            *
            *
            *
            List<Job> day1_p1 = capable[0].FindAll(x => x.job_dates == 1);
            List<Job> day1_p2 = capable[1].FindAll(x => x.job_dates == 1);
            List<Job> day1_p3 = capable[2].FindAll(x => x.job_dates == 1);

            //print class
            foreach (Job obj in day1_p1)
            {
                Console.WriteLine("Here is the day1 for p1");
                Console.WriteLine(obj.jobID);
            }
            
            //sort day1 job by windows_start
            List<Job> sol_day1_p1 = day1_p1.OrderBy(o => o.win_start).ToList();
            List<Job> sol_day1_p2 = day1_p2.OrderBy(o => o.win_start).ToList();
            List<Job> sol_day1_p3 = day1_p3.OrderBy(o => o.win_start).ToList();
            
            foreach (Job obj in sol_day1_p1)
            {
                Console.WriteLine("Here is sorted day1 for p1");
                Console.WriteLine(obj.jobID);
            }

           
            Console.Write(sol_day1_p1.Count);
            //remove the infeasible jobs that overlap in job duration
            for (int i = 0; i < sol_day1_p1.Count()-1; i++)
            {
                Job temp0 = sol_day1_p1[i];
                Console.WriteLine("p1 i = {0}", temp0.jobID);
                Job temp1 = sol_day1_p1[i + 1];
                Console.WriteLine("p1 i+1 = {0}", temp1.jobID);

                if (sol_day1_p1[i].win_end > sol_day1_p1[i+1].win_start)
                {
                    Job temp = sol_day1_p1[i + 1];
                    sol_day1_p1.Remove(sol_day1_p1[i+1]);
                    Console.WriteLine("p1 remove {0}", temp.jobID);
                    Console.WriteLine("count:{0}",sol_day1_p1.Count);
                }
            }



            for (int i = 0; i < sol_day1_p2.Count - 1; i++)
            {
                if (sol_day1_p2[i].win_end > sol_day1_p2[i + 1].win_start)
                {
                    //sol_day1_p2.Remove(sol_day1_p2[i + 1]);
                    Console.WriteLine("p2 remove {0}", sol_day1_p2[i + 1].jobID);
                }
            }
            for (int i = 0; i < sol_day1_p3.Count - 1; i++)
            {

                Job temp0 = sol_day1_p3[i];
                Console.WriteLine("p3 i = {0}", temp0.jobID);
                Job temp1 = sol_day1_p3[i + 1];
                Console.WriteLine("p3 i+1 = {0}", temp1.jobID);

                if (sol_day1_p3[i].win_end > sol_day1_p3[i + 1].win_start)
                {
                    
                    Job temp2 = sol_day1_p3[i + 1];
                    sol_day1_p3.Remove(sol_day1_p3[i + 1]);
                    Console.WriteLine("p1 remove {0}", temp2.jobID);
                }
            }
            Console.Write("Duty for p1: ");
            foreach (Job obj in sol_day1_p1)
            {
                Console.WriteLine(obj.jobID);
            }
            Console.Write("Duty for p2: ");
            foreach (Job obj in sol_day1_p2)
            {
                Console.WriteLine(obj.jobID);
            }
            Console.Write("Duty for p3: ");
            foreach (Job obj in sol_day1_p3)
            {
                Console.WriteLine(obj.jobID);
            }
             */





        }
    }
}
