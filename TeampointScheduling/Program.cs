using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TeampointScheduling;
using System.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions; // for Regex.Split
using System.Configuration;
using System.Collections.Specialized;

namespace TeampointScheduling
{

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
    }

    public class Duty //in use
    {
        public string date;
        public List<Jobs> dutyset;
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

    public class assign // in use
    { 
        public Jobs job;
        public int jobID;
        public int drvTime;
        public int drv_str;
        public int job_str; 
        public int job_end;
        public int drv_back;
        public int totalwork;
        public int totaldrive;

    }


    public class available  // in use
    {
        public int str;
        public int end;
        public int duration;   
    }

    public class traveltime 
    {
        public string locFrom;
        public string locTo;
        public int duration;
    }

    public class driving //in use
    {
        public Jobs job;
        public int drvTime;
        public int drvstr; //the time staff need to start drive
        public int jobstr; //the start of the job window 
        public int workstr; //the actual job start time
        public int drvback; //time that driving start
    }


    class Program
    {
        

        static void Main(string[] args)
        {
            //Configuration 
            
            // Read a particular key from the config file 
            //string sAttr;
            //sAttr = ConfigurationManager.AppSettings.Get("server");
            //Console.WriteLine("The value of Key0: " + sAttr);

            // Read all the keys from the config file
            NameValueCollection sAll;
            sAll = ConfigurationManager.AppSettings;

            foreach (string s in sAll.AllKeys)
                Console.WriteLine("key: " + s + ", value: " + sAll.Get(s));

            //server
            string server = sAll.Get("server");
            string port = sAll.Get("port");
            string database = sAll.Get("database");
            string userid = sAll.Get("userid");
            string password = sAll.Get("password");
            //job table
            string job_table = sAll.Get("job_table");
            string j_jobid = sAll.Get("job_table_jobid"); 
            string j_priority = sAll.Get("job_table_priority");
            string j_duration = sAll.Get("job_table_duration");
            string j_dates = sAll.Get("job_table_dates");
            string j_windows = sAll.Get("job_table_windows");
            string j_tags = sAll.Get("job_table_tags");
            //operative table    
            string operative_table = sAll.Get("operative_table");
            string o_personid = sAll.Get("operative_table_personid");
            string o_tags = sAll.Get("operative_table_tags");
            string o_dates = sAll.Get("operative_table_dates");
            string o_maxdriving = sAll.Get("operative_table_max_driving");
            //travel time table
            string travel_time_table = sAll.Get("travel_time_table");
            string t_fromloc = sAll.Get("travel_time_table_from");
            string t_toloc = sAll.Get("travel_time_table_to");
            string t_duration = sAll.Get("travel_time_table_duration");            
            //result table
            string result_table = sAll.Get("result_table");           
            string r_date = sAll.Get("result_table_date");
            string r_personid = sAll.Get("result_table_personid");
            string r_jobid = sAll.Get("result_table_jobid");
            string r_starttime = sAll.Get("result_table_start_time");
            string r_endtime = sAll.Get("result_table_end_time");
            string r_duration = sAll.Get("result_table_duration");
            string r_tasktype = sAll.Get("result_table_task_type");
            //assigning iteration limit
            string iteration_limit = sAll.Get("iteration_limit");
            Console.ReadLine();

   

            // =========================START FROM HERE==============================

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            //create list to store data from MYSQL
            List<Jobs> jobset = new List<Jobs>();
            List<Staff> staffset = new List<Staff>();
            List<Duty> dateDuty = new List<Duty>();
            List<traveltime> trvtime = new List<traveltime>();

            //operative data
            List<int> personid = new List<int>();
            List<int> max_minutes = new List<int>();
            List<int> max_miles = new List<int>();
            List<string> dayoff = new List<string>();
            List<int> worker_tags = new List<int>();

            //For searching travel time
            Dictionary<(int, int), int> staffjob = new Dictionary<(int, int), int>();
            Dictionary<(int, int), int> between = new Dictionary<(int, int), int>();
            Dictionary<(int, int), int> jobstaff = new Dictionary<(int, int), int>();
            
            //dates range
            List<string> horizon = new List<string>();
            
            //the max windows number(for assigning jobs)
            int winNum = 0; 

            //Clear MySQL result
            string clear = $@"Server = {server}; Port = {port}; Database = {database} ; Uid = {userid}; Pwd = {password} ; default command timeout = 0";
            using (var conn = new MySqlConnection(clear))
            {
                conn.Open();

                string delete = $"DELETE FROM {database}.{result_table};";
                using (var cmd = new MySqlCommand(delete, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }

                string id = $"ALTER TABLE {database}.{result_table} AUTO_INCREMENT = 1;";
                using (var cmd = new MySqlCommand(id, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }

                conn.Close();
            }


            //read jobs from MySQL
            string cs = $@"Server = {server}; Port = {port}; Database = {database} ; Uid = {userid}; Pwd = {password} ; default command timeout = 0";
            using (var conn = new MySqlConnection(cs))
            {
                conn.Open();

                string sql = $"SELECT {j_jobid}, {j_priority}, {j_duration}, {j_dates}, {j_windows}, {j_tags} FROM {database}.{job_table}";
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
            string cs1 = $@"Server = {server}; Port = {port}; Database = {database} ; Uid = {userid}; Pwd = {password} ; default command timeout = 0";
            using (var conn = new MySqlConnection(cs1))
            {
                conn.Open();

                string sql = $"SELECT {o_personid}, {o_tags}, {o_dates}, {o_maxdriving} FROM {database}.{operative_table}";
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
                            string[] stringSeparators = new string[] { "date", };
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
                                max_minutes_.Add(sub*60); // minutes to seconds
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

            Console.WriteLine("Loading travel time ...");
            //read traveltime from MySQL
            string trv = $@"Server = {server}; Port = {port}; Database = {database} ; Uid = {userid}; Pwd = {password} ; default command timeout = 0";
            using (var conn = new MySqlConnection(trv))
            {
                conn.Open();
                
                string sql = $"SELECT {t_fromloc} , {t_toloc}, {t_duration} FROM {database}.{travel_time_table}";                
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //travel time list
                            string locFrom = reader.GetString(0);
                            string locTo = reader.GetString(1);
                            int travel = int.Parse(reader.GetString(2));
                            //Console.WriteLine("{0},{1},{2}",locFrom, locTo, travel );
                            trvtime.Add(new traveltime() {locFrom = locFrom, locTo = locTo, duration = travel });
                            
                            // dictionary
                            // from staff to job
                            if (locFrom.Contains("T-") && !(locTo.Contains("T-")))
                            {
                                int staff = int.Parse(locFrom.TrimStart('T', '-'));
                                int to = int.Parse(locTo);
                                staffjob.Add((staff, to), travel);
                            }
                            //from job to staff
                            else if (!locFrom.Contains("T-") && (locTo.Contains("T-")))
                            {
                                int from = int.Parse(locFrom);
                                int staff = int.Parse(locTo.TrimStart('T', '-'));                                
                                jobstaff.Add((from, staff), travel);
                            }
                            //between two jobs
                            else if (!locFrom.Contains("T-") && !(locTo.Contains("T-")))
                            {
                                int from = int.Parse(locFrom);
                                int to = int.Parse(locTo);
                                between.Add((from, to), travel);
                            }



                        }
                    }
                }
            }


            //==========================Find feasible jobs for specific dates===================================

            List<Jobs> assignedjob = new List<Jobs>(); //To remove the assigned jobs in the capable list
            int removeNum = 0; // To count how many jobs in capable_ list has been removed since the job is already assigned
            for (int d = 0; d < horizon.Count(); d++)
            {
                //store the possible job for each staff
                List<Jobs>[] capable_ = new List<Jobs>[staffset.Count()];

                //store the assignment for each staff
                List<assign>[] initial = new List<assign>[staffset.Count()];

                //store the available windows for each staff
                List<available>[] available_wins = new List<available>[staffset.Count()];

                //sort assignjob, to decrease loop number                     
                assignedjob = assignedjob.OrderBy(o => o.job_dates[0]).ToList();

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
                                //2.3 Assigned check: check if the job is already assigned
                                bool a = assignedjob.Exists(x => x.jobID == dateDuty[d].dutyset[j].jobID);

                                if(!a)
                                {
                                    //2.4 Windows check
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
                    Console.WriteLine("\n");
                    //find the staff work start and end on a specific date
                    //index == -1 when the system can't find index of date

                    //sort assignjob, to decrease loop number                    
                    assignedjob = assignedjob.OrderBy(o => o.job_dates[0]).ToList();


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
                            totaldrive = 0
                        });
                        Console.WriteLine("staff {0} doesn't work on the date", staffset[i].personID);
                    }
                    else
                    {
                        if (capable_[i].Count() == 0)
                            Console.WriteLine("staff {0} works, but has no capable job on the date", staffset[i].personID);

                        else if (capable_[i].Count() > 0)
                        {
                            //Part0: Rmove all assigned jobs                            
                            foreach (Jobs job in assignedjob)
                            {

                                for (int k = 0; k < capable_[i].Count(); k++)
                                {
                                    if (capable_[i][k].jobID == job.jobID)
                                    {
                                        removeNum = removeNum + 1;
                                        //Console.WriteLine("{0} remove {1}", staffset[i].personID, capable_[i][k].jobID);
                                        capable_[i].RemoveAt(k);
                                        k--;
                                    }
                                }
                            }



                            int num = initial[i].Count();
                            int temp = 0;


                            while (capable_[i].Count() > 0)
                            {
                                
                                //Part1: Update available Windows  ======================================
                                int staff_str = staffset[i].work_str[index];
                                int staff_end = staffset[i].work_end[index];

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
                                    //Console.WriteLine("\nadd staff {0} doesn't work on the date", staffset[i].personID);

                                }

                                //staff work on that day and not assigned any job
                                else if (index >= 0 && initial[i].Count() == 0)
                                {

                                    available_wins[i].Add(new available
                                    {
                                        str = staff_str,
                                        end = staff_end,
                                        duration = staff_end - staff_str,
                                    });

                                    //Console.WriteLine("\nadd staff {0} available from {1} to {2}, no duty ", staffset[i].personID, staff_str, staff_end);

                                }

                                //staff work on that day and have duty
                                else if (index >= 0 && initial[i].Count() > 0)
                                {
                                    //Console.WriteLine("\nstaff {0} works and have duty ", staffset[i].personID);


                                    //Console.WriteLine("\nstaffstr: {0}", staff_str);
                                    //for (int n = 0; n < initial[i].Count();n++)
                                    //Console.WriteLine("job {0} str:{1} end:{2} dur: {3}", initial[i][n].jobID, initial[i][n].job_str, initial[i][n].job_end, initial[i][n].job.duration );

                                    int dutyNum = initial[i].Count();

                                    /*
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
                                    */
                                    // for the last job
                                    if (initial[i][dutyNum - 1].job_end < staff_end)
                                    {
                                        available_wins[i].Add(new available
                                        {
                                            str = initial[i][dutyNum - 1].job_end,
                                            end = staff_end,
                                            duration = staff_end - initial[i][dutyNum - 1].job_end,
                                        });

                                        //Console.WriteLine("add staff {0} available from {1} to {2}", staffset[i].personID, initial[i][dutyNum - 1].job_end, staff_end);

                                    }

                                }
                                //Sort available windows 
                                available_wins[i] = available_wins[i].OrderBy(o => o.str).ToList();
                                //============== End of Update available ======================

                                //Part2: Assign the first duty  ======================================

                                //sort capable list
                                capable_[i] = capable_[i].OrderBy(o => o.win_str[0]).ToList();


                                /*
                                //(1) Option 1: random assign     
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
                                        assignedjob.Add(capable_[i][rnd]);
                                        capable_[i].RemoveAt(rnd);

                                    }                                   
                                }
                                */
                                //


                                //Option 2: assign job with earliest driving + start time the job as first

                                if (initial[i].Count() == 0) //first duty
                                {
                                    // randomly assign a job from possible job list
                                    if (capable_[i].Count() > 0) //staff has possible work 
                                    {
                                        List<driving> firstDrv = new List<driving>();
                                        //Find driving time from operator's initial location to all the other jobs'location
                                        string s = staffset[i].personID.ToString();
                                        

                                        for (int j = 0; j < capable_[i].Count(); j++)
                                        {
                                            //Search driving time: dictionary

                                            int id = staffset[i].personID;
                                            int job = capable_[i][j].jobID;
                                            int result = 0; 
                                            int find = 0;
                                            
                                            if (staffjob.TryGetValue((id, job), out result))
                                            {
                                                find = result;
                                            }
                                            else if (jobstaff.TryGetValue((job, id), out result))
                                            {
                                                find = result;
                                            }
                                            else 
                                            {
                                                Console.WriteLine("Key staff {0}, job {1} driving time is not found.", id, job);
                                            }



                                            
                                            if (find != 0)
                                            {
                                                //multiple windows
                                                for (int k = 0; k < capable_[i][j].win_str.Count(); k++)
                                                {                                                    
                                                    int check = capable_[i][j].win_str[k] - find; //Jobstr-Drive
                                                    int workstr = check; int drvstr = capable_[i][j].win_str[k];
                                                    //Case1: job start time equals 1 means anytime 
                                                    if (capable_[i][j].win_str[k] == 1)
                                                    {
                                                        drvstr = staff_str;             
                                                        workstr = staff_str + find;
                                                    }

                                                    //Case2: workstr is in the job window
                                                    if (check < staff_str)  // Jobstr-Drive < staffstr
                                                    {
                                                        // if staffstr + drive + work <= jobend
                                                        if (staff_str + find + capable_[i][j].duration <= capable_[i][j].win_end[k])
                                                        {
                                                            drvstr = staff_str;
                                                            workstr = staff_str + find;
                                                        }
                                                        else
                                                        {
                                                            continue; //not able to do this job with this window
                                                        }
                                                    }
                                                    //Case3: workstr in the begining of job window
                                                    else if (check >= staff_str) //Jobstr - Duration >= staffstr
                                                    {
                                                        drvstr = check;
                                                        workstr = capable_[i][j].win_str[k];
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("\nNEW driving case not included yet!");
                                                    }
                                                    //Console.WriteLine("\nadd possible firstDrv");
                                                    firstDrv.Add(new driving { job = capable_[i][j], drvTime = find, jobstr = capable_[i][j].win_str[k], drvstr = drvstr, workstr = workstr, drvback = find });

                                                }

                                            }

                                        }

                                        //sort jobs by start time considering driving time
                                        firstDrv = firstDrv.OrderBy(x => x.workstr).ToList();
                                        Console.WriteLine("====================Staff {0} Scheduling Result on {1}========================", staffset[i].personID, horizon[d]);
                                        Console.WriteLine("staffstr:{0} staffend:{1} maxwork:{2}", staff_str, staff_end, staffset[i].max_minutes[index]);


                                        for (int k = 0; k < firstDrv.Count(); k++)
                                        {
                                            //Console.WriteLine("staff{0} job:{1} jobstr:{2} drivestr:{3} drive:{4} workstr:{5}", staffset[i].personID, firstDrv[k].job.jobID, firstDrv[k].jobstr, firstDrv[k].drvstr, firstDrv[k].drvTime, firstDrv[k].workstr);
                                        }
                                        if(firstDrv.Count() > 0)
                                        {
                                            int end = firstDrv[0].workstr + firstDrv[0].job.duration;

                                            initial[i].Add(new assign
                                            {
                                                job = firstDrv[0].job,
                                                jobID = firstDrv[0].job.jobID,
                                                drvTime = firstDrv[0].drvTime,
                                                drv_str = firstDrv[0].drvstr,
                                                job_str = firstDrv[0].workstr,
                                                job_end = end,
                                                drv_back = firstDrv[0].drvback,
                                                totalwork = firstDrv[0].job.duration,
                                                totaldrive = firstDrv[0].drvTime,
                                            });
                                            Console.WriteLine("staff {0} fisrt assign {1} drvstr:{2} drvTime:{3} workstr:{4} workTime:{5} end:{6}, totalwork:{7} totaldrive:{8}",
                                                staffset[i].personID,
                                                firstDrv[0].job.jobID,
                                                firstDrv[0].drvstr,
                                                firstDrv[0].drvTime,
                                                firstDrv[0].workstr,
                                                firstDrv[0].job.duration,
                                                end,
                                                firstDrv[0].job.duration,
                                                firstDrv[0].drvTime

                                                );

                                            assignedjob.Add(firstDrv[0].job);

                                            //delete all records
                                            string write = $@"Server = {server}; Port = {port}; Database = {database} ; Uid = {userid}; Pwd = {password} ; default command timeout = 0";
                                            using (var conn = new MySqlConnection(write))
                                            {
                                                conn.Open();

                                                //drive
                                                string drv = $"insert into {database}.{result_table} ({r_date}, {r_personid}, {r_jobid}, {r_starttime}, {r_endtime}, {r_duration}, {r_tasktype}) values( @date, @person, @job, @str, @end, @dur, @type);";
                                                using (var cmd = new MySqlCommand(drv, conn))
                                                {
                                                    cmd.Parameters.AddWithValue("@date", horizon[d]);
                                                    cmd.Parameters.AddWithValue("@person", staffset[i].personID);
                                                    cmd.Parameters.AddWithValue("@job", firstDrv[0].job.jobID);
                                                    cmd.Parameters.AddWithValue("@str", firstDrv[0].drvstr);
                                                    cmd.Parameters.AddWithValue("@end", firstDrv[0].workstr);
                                                    cmd.Parameters.AddWithValue("@dur", firstDrv[0].drvTime);
                                                    cmd.Parameters.AddWithValue("@type", 1);
                                                    using (var reader = cmd.ExecuteReader())
                                                    {
                                                        while (reader.Read())
                                                        {

                                                        }
                                                    }
                                                }

                                                //work;
                                                string work = $"insert into {database}.{result_table} ({r_date}, {r_personid}, {r_jobid}, {r_starttime}, {r_endtime}, {r_duration}, {r_tasktype}) values(@date, @person, @job, @str, @end, @dur, @type);";
                                                using (var cmd = new MySqlCommand(work, conn))
                                                {
                                                    cmd.Parameters.AddWithValue("@date", horizon[d]);
                                                    cmd.Parameters.AddWithValue("@person", staffset[i].personID);
                                                    cmd.Parameters.AddWithValue("@job", firstDrv[0].job.jobID);
                                                    cmd.Parameters.AddWithValue("@str", firstDrv[0].workstr);
                                                    cmd.Parameters.AddWithValue("@end", end);
                                                    cmd.Parameters.AddWithValue("@dur", firstDrv[0].job.duration);
                                                    cmd.Parameters.AddWithValue("@type", 2);
                                                    using (var reader = cmd.ExecuteReader())
                                                    {
                                                        while (reader.Read())
                                                        {

                                                        }
                                                    }
                                                }


                                                conn.Close();
                                            }


                                            //remove the assigned job from the staff capable list
                                            int x = capable_[i].IndexOf(firstDrv[0].job);
                                            capable_[i].RemoveAt(x);

                                            
                                        }


                                    }
                                }

                                /*//Option 2: assign job with earliest driving + start time the job as first

                                if (initial[i].Count == 0) //first duty
                                {
                                    // randomly assign a job from possible job list
                                    if (capable_[i].Count() > 0) //staff has possible work 
                                    {

                                        List<driving> firstDrv = new List<driving>();

                                        //Find driving time from operator's initial location to all the other jobs'location
                                        string s = staffset[i].personID.ToString();
                                        for (int j = 0; j < capable_[i].Count(); j++)
                                        {

                                            //search driving time
                                            List<traveltime> find = GetDrivingTime(trvtime, $"T-{s}", capable_[i][j].jobID.ToString());
                                            //multiple windows
                                            for (int k = 0; k < capable_[i][j].win_str.Count(); k++)
                                            {
                                                if (find.Count() != 0)
                                                {
                                                    int str; // driving start time
                                                    if (capable_[i][j].win_str[k] == 1)
                                                        str = staffset[i].work_str[0];
                                                    else
                                                        str = capable_[i][j].win_str[k] - find[0].duration;

                                                    firstDrv.Add(new driving { job = capable_[i][j], drvTime = find[0].duration, str = str });

                                                }
                                                else if (find.Count() == 0)
                                                {
                                                    //search driving time from the opposite direction
                                                    find = GetDrivingTime(trvtime, capable_[i][j].jobID.ToString(), $"T-{s}");
                                                    if (find.Count() == 0)
                                                        Console.WriteLine("No driving time data from staff {0} to {1}", s, capable_[i][j].jobID);
                                                    else
                                                    {

                                                        int str; // driving start time
                                                        if (capable_[i][j].win_str[k] == 1) // job start time equals 1 means anytime 
                                                            str = staffset[i].work_str[0];
                                                        else
                                                            str = capable_[i][j].win_str[k] - find[0].duration;


                                                        firstDrv.Add(new driving { job = capable_[i][j], drvTime = find[0].duration, str = str });

                                                    }
                                                }
                                            }


                                        }


                                        // Sorted by Value  

                                        Console.WriteLine("====== Driving time result =======");
                                        Console.WriteLine("staff start:{0}", staffset[i].work_str[0]);




                                        //sort jobs by start time considering driving time
                                        firstDrv = firstDrv.OrderBy(x => x.drvTime).ToList();
                                        //foreach( driving x in firstDrv)
                                        //Console.WriteLine("staff{0} strdrive:{1} job:{2} str:{3} drive:{4} actual work str:{5}", staffset[i].personID, x.str, x.job.jobID, x.job.win_str[0] , x.drvTime,(x.str + x.drvTime));
                                        for (int k = 0; k < firstDrv.Count(); k++)
                                        {
                                            Console.WriteLine("Total");
                                            Console.WriteLine("staff{0} job:{1} strdrive:{2} drive:{3} jobstr:{4} actual job str:{5}", staffset[i].personID, firstDrv[0].job.jobID, firstDrv[k].str, firstDrv[k].drvTime, firstDrv[k].job.win_str[0], firstDrv[k].str + firstDrv[k].drvTime);
                                        }
                                        for (int k = 0; k < firstDrv.Count(); k++)
                                        {
                                            //the official job start window is not early
                                            if (staffset[i].work_str[0] <= firstDrv[k].str)
                                            {

                                                int str = firstDrv[k].str + firstDrv[k].drvTime;
                                                int end = str + firstDrv[k].job.duration;
                                                Console.WriteLine("staff{0} job:{1} strdrive:{2} drive:{4} jobstr:{3} actual job str:{5}", staffset[i].personID, firstDrv[k].job.jobID, firstDrv[k].str, firstDrv[k].drvTime, firstDrv[k].job.win_str[0], str);
                                                initial[i].Add(new assign
                                                {
                                                    job = firstDrv[k].job,
                                                    jobID = firstDrv[k].job.jobID,
                                                    job_str = str,
                                                    job_end = end,
                                                    totalwork = firstDrv[k].job.duration,
                                                });
                                                Console.WriteLine("staff {0} assign {1} strDrv:{2} driving:{3} strwork:{4} end:{5}", staffset[i].personID, firstDrv[k].job.jobID, firstDrv[k].str, firstDrv[k].drvTime, str, end);
                                                Console.WriteLine("drive time: {0}, work time: {1}\n", firstDrv[k].drvTime, firstDrv[k].job.duration);
                                                //remove the assigned job from the staff capable list
                                                assignedjob.Add(firstDrv[k].job);

                                                int x = capable_[i].IndexOf(firstDrv[k].job);
                                                capable_[i].RemoveAt(x);

                                                break;
                                            }
                                            // if job cannot start at its window str
                                            else if (staffset[i].work_str[0] + firstDrv[k].drvTime + firstDrv[k].job.duration <= firstDrv[k].job.win_end[0])
                                            {

                                                int str = staffset[i].work_str[0] + firstDrv[k].drvTime;
                                                int end = str + firstDrv[k].job.duration;
                                                Console.WriteLine("staff{0} job:{1} strdrive:{2} drive:{4} jobstr:{3} actual job str:{5}", staffset[i].personID, firstDrv[k].job.jobID, firstDrv[k].str, firstDrv[k].drvTime, firstDrv[k].job.win_str[0], str);
                                                initial[i].Add(new assign
                                                {
                                                    job = firstDrv[k].job,
                                                    jobID = firstDrv[k].job.jobID,
                                                    job_str = str,
                                                    job_end = end,
                                                    totalwork = firstDrv[k].job.duration,
                                                });
                                                Console.WriteLine("staff {0} assign {1} strDrv:{2} driving:{3} strwork:{4} end:{5}", staffset[i].personID, firstDrv[k].job.jobID, staffset[i].work_str[0], firstDrv[k].drvTime, str, end);
                                                Console.WriteLine("drive time: {0}, work time: {1}\n", firstDrv[k].drvTime, firstDrv[k].job.duration);
                                                //remove the assigned job from the staff capable list
                                                assignedjob.Add(firstDrv[k].job);

                                                int x = capable_[i].IndexOf(firstDrv[k].job);
                                                capable_[i].RemoveAt(x);
                                                break;
                                            }

                                        }

                                    }
                                } */


                                //Part3: Assign the rest of jobs  ======================================    
                                else if (initial[i].Count() > 0) //after first duty, to assign duty based on available list
                                {
                                    if (capable_[i].Count() > 0) //staff has possible work 
                                    {
                                        List<driving> JobDrv = new List<driving>();
                                        //Find driving time from one location to all the other jobs'location

                                        num = initial[i].Count();
                                        string s = staffset[i].personID.ToString();
                                        string duty = initial[i][num - 1].jobID.ToString();





                                        for (int j = 0; j < capable_[i].Count(); j++)
                                        {
                                            //Search driving time dictionary

                                            int id = staffset[i].personID;
                                            int job = capable_[i][j].jobID;
                                            int prejob = initial[i][num - 1].jobID;
                                            int result = 0;
                                            int find = 0;
                                            int back = 0;

                                            //between jobs driving time
                                            if (between.TryGetValue((prejob, job), out result))
                                            {
                                                find = result;
                                            }
                                            else if (between.TryGetValue((job, prejob), out result))
                                            {
                                                find = result;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Key job {0} and job {1} driving time is not found.", prejob, job);
                                            }

                                            //return to initial position driving time
                                            if (jobstaff.TryGetValue((job, id), out result))
                                            {
                                                back = result;
                                            }
                                            else if (staffjob.TryGetValue((id, job), out result))
                                            {
                                                back = result;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Key staff {0}, job {1} driving time is not found.", id, job);
                                            }


                                            
                                            if (find != 0)
                                            {
                                                //multiple windows
                                                for (int k = 0; k < capable_[i][j].win_str.Count(); k++)
                                                {
                                                    
                                                    int check = capable_[i][j].win_str[k] - find; //Jobstr-Drive
                                                    int workstr = check; int drvstr = capable_[i][j].win_str[k];


                                                    //Case1: job start time equals 1 means anytime && Case2: workstr is in the job window
                                                    if (capable_[i][j].win_str[k] == 1 || check < initial[i][num - 1].job_end)  // Jobstr-Drive < end of the last job
                                                    {
                                                        // if staffstr + drive + work <= jobend                                                        
                                                        if (initial[i][num - 1].job_end + find + capable_[i][j].duration <= capable_[i][j].win_end[k])
                                                        {
                                                            drvstr = initial[i][num - 1].job_end;
                                                            workstr = initial[i][num - 1].job_end + find;
                                                            //Console.WriteLine("case 1 & 2 ");
                                                        }
                                                        else
                                                        {
                                                            //Console.WriteLine("not able to do this job with this window");
                                                            continue; //not able to do this job with this window
                                                        }
                                                    }
                                                    //Case3: workstr in the begining of job window
                                                    else if (check >= initial[i][num - 1].job_end) //Jobstr - Duration >= staffstr
                                                    {
                                                        drvstr = check;
                                                        workstr = capable_[i][j].win_str[k];
                                                        //Console.WriteLine("case 3");
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("\nNEW driving case not included yet!");
                                                    }

                                                    //staff min work on the date
                                                    int maxwork = staffset[i].max_minutes[index];
                                                    int total = capable_[i][j].duration + initial[i][num - 1].totalwork;

                                                    //staff min drive time on the date
                                                    int maxdrive = staffset[i].max_miles;                                                    
                                                    int totaldrive = find + initial[i][num - 1].totaldrive;

                                                    //To check (1)staff have time to drive back to the job center (2) total worktime <= max work time
                                                    //(3) total drive minutes <= max drive time
                                                    
                                                    if (workstr + capable_[i][j].duration + back <= staff_end && total <= maxwork && totaldrive <= maxdrive)
                                                    {

                                                        //Console.WriteLine("\nadd possible JobDrv");
                                                        
                                                        {
                                        
                                                            JobDrv.Add(new driving { job = capable_[i][j], drvTime = find, jobstr = capable_[i][j].win_str[k], drvstr = drvstr, workstr = workstr, drvback = back});
                                                        }
                                                    }
                                                    //else if (total > maxwork)
                                                    //Console.WriteLine("exceed max work time");
                                                    //else if (workstr + capable_[i][j].duration + back[0].duration > staff_end)
                                                    //Console.WriteLine("no time to drive back");

                                                }

                                            }
                                            else
                                                Console.WriteLine("Can't find driving time.");

                                        }

                                        //sort jobs by start time considering driving time
                                        JobDrv = JobDrv.OrderBy(x => x.workstr).ToList();

                                        //Console.WriteLine("====== Driving time result =======");
                                        //Console.WriteLine("staffstr:{0} staffend:{1} maxwork:{2}", staff_str, staff_end, staffset[i].max_minutes[index]);


                                        //for (int k = 0; k < JobDrv.Count(); k++)
                                        {
                                            //Console.WriteLine("staff{0} job:{1} jobstr:{2} drivestr:{3} drive:{4} workstr:{5}", staffset[i].personID, JobDrv[0].job.jobID, JobDrv[0].jobstr, JobDrv[0].drvstr, JobDrv[0].drvTime, JobDrv[0].workstr);
                                        }

                                        //the previous assign method


                                        if (JobDrv.Count() > 0)
                                        {
                                            int end = JobDrv[0].workstr + JobDrv[0].job.duration;
                                            initial[i].Add(new assign
                                            {
                                                job = JobDrv[0].job,
                                                jobID = JobDrv[0].job.jobID,
                                                drvTime = JobDrv[0].drvTime,
                                                drv_str = JobDrv[0].drvstr,
                                                job_str = JobDrv[0].workstr,
                                                job_end = end,
                                                drv_back = JobDrv[0].drvback,
                                                totalwork = initial[i][num - 1].totalwork + JobDrv[0].job.duration,
                                                totaldrive = initial[i][num - 1].totaldrive + JobDrv[0].drvTime,
                                            });


                                            Console.WriteLine("staff {0} assign {1} drvstr:{2} drvTime:{3} workstr:{4} workTime:{5} end:{6} totalwork:{7} totaldrive:{8}",
                                                staffset[i].personID,
                                                JobDrv[0].job.jobID,
                                                JobDrv[0].drvstr,
                                                JobDrv[0].drvTime,
                                                JobDrv[0].workstr,
                                                JobDrv[0].job.duration,
                                                end,
                                                initial[i][num - 1].totalwork + JobDrv[0].job.duration,
                                                initial[i][num - 1].totaldrive + JobDrv[0].drvTime
                                                );

                                            assignedjob.Add(JobDrv[0].job);

                                            //write back to database
                                            string write = $@"Server = {server}; Port = {port}; Database = {database} ; Uid = {userid}; Pwd = {password} ; default command timeout = 0";
                                            using (var conn = new MySqlConnection(write))
                                            {
                                                conn.Open();

                                                //drive
                                                string drv = $"insert into {database}.{result_table} ({r_date}, {r_personid}, {r_jobid}, {r_starttime}, {r_endtime}, {r_duration}, {r_tasktype}) values(@date, @person, @job, @str, @end, @dur, @type);";
                                                using (var cmd = new MySqlCommand(drv, conn))
                                                {
                                                    cmd.Parameters.AddWithValue("@date", horizon[d]);
                                                    cmd.Parameters.AddWithValue("@person", staffset[i].personID);
                                                    cmd.Parameters.AddWithValue("@job", JobDrv[0].job.jobID);
                                                    cmd.Parameters.AddWithValue("@str", JobDrv[0].drvstr);
                                                    cmd.Parameters.AddWithValue("@end", JobDrv[0].workstr);
                                                    cmd.Parameters.AddWithValue("@dur", JobDrv[0].drvTime);
                                                    cmd.Parameters.AddWithValue("@type", 1);
                                                    using (var reader = cmd.ExecuteReader())
                                                    {
                                                        while (reader.Read())
                                                        {

                                                        }
                                                    }
                                                }

                                                //work;
                                                string work = "insert into data1.schedule_results (date, personid, jobid, start_time, end_time, duration, task_type) values( @date, @person, @job, @str, @end, @dur, @type);";
                                                using (var cmd = new MySqlCommand(work, conn))
                                                {
                                                    cmd.Parameters.AddWithValue("@date", horizon[d]);
                                                    cmd.Parameters.AddWithValue("@person", staffset[i].personID);
                                                    cmd.Parameters.AddWithValue("@job", JobDrv[0].job.jobID);
                                                    cmd.Parameters.AddWithValue("@str", JobDrv[0].workstr);
                                                    cmd.Parameters.AddWithValue("@end", end);
                                                    cmd.Parameters.AddWithValue("@dur", JobDrv[0].job.duration);
                                                    cmd.Parameters.AddWithValue("@type", 2);
                                                    using (var reader = cmd.ExecuteReader())
                                                    {
                                                        while (reader.Read())
                                                        {

                                                        }
                                                    }
                                                }


                                                conn.Close();
                                            }

                                            //remove the assigned job from the staff capable list
                                            int x = capable_[i].IndexOf(JobDrv[0].job);
                                            capable_[i].RemoveAt(x);

                                        }

                                        


                                        /* Without driving time
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
                                                        assignedjob.Add(capable_[i][rnd]);
                                                        capable_[i].RemoveAt(rnd);

                                                        assigned = true;
                                                        break;
                                                    }
                                                }// multiple window

                                            } //available window is long enough 

                                            if (assigned)
                                                break;

                                        }//for every available windows
                                        
                                        */

                                    }//staff has possible work
                                }//after first duty to assign duty 

                                //============== End of assigning jobs ======================
                                //Sort initial assignment
                                initial[i] = initial[i].OrderBy(o => o.job_str).ToList();




                                temp = temp + 1;
                                if (temp > int.Parse(iteration_limit))
                                {
                                    break;
                                }

                            }//do while

                            if (initial[i].Count() > 0)
                            {
                                //write the last driving to database
                                //Last driving end time
                                int endtime = initial[i][initial[i].Count() - 1].job_end + initial[i][initial[i].Count() - 1].drv_back;

                                string returnLoc = $@"Server = {server}; Port = {port}; Database = {database} ; Uid = {userid}; Pwd = {password} ; default command timeout = 0";
                                using (var conn = new MySqlConnection(returnLoc))
                                {
                                    conn.Open();

                                    string drv = $"insert into {database}.{result_table} ({r_date}, {r_personid}, {r_jobid}, {r_starttime}, {r_endtime}, {r_duration}, {r_tasktype}) values(@date, @person, @job, @str, @end, @dur, @type);";
                                    using (var cmd = new MySqlCommand(drv, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@date", horizon[d]);
                                        cmd.Parameters.AddWithValue("@person", staffset[i].personID);
                                        cmd.Parameters.AddWithValue("@job", initial[i][initial[i].Count() - 1].jobID);
                                        cmd.Parameters.AddWithValue("@str", initial[i][initial[i].Count() - 1].job_end);
                                        cmd.Parameters.AddWithValue("@end", endtime);
                                        cmd.Parameters.AddWithValue("@dur", initial[i][initial[i].Count() - 1].drv_back);
                                        cmd.Parameters.AddWithValue("@type", 1);
                                        using (var reader = cmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {

                                            }
                                        }
                                    }
                                    conn.Close();
                                }
                            }
                            //for (int n = 0; n < initial[i].Count(); n++)
                            //Console.WriteLine("job {0} drvstr:{1} workstr:{2} end:{3}", initial[i][n].jobID, initial[i][n].drv_str, initial[i][n].job_str, initial[i][n].job_end);
                            for (int n = 0; n < available_wins[i].Count(); n++)
                                Console.WriteLine("\nstaff {0} is available from {1} to {2}", staffset[i].personID, available_wins[i][n].str, available_wins[i][n].end);
                            Console.Write("staff {0} capable list:", staffset[i].personID);
                            for (int c = 0; c < capable_[i].Count(); c++)
                                Console.Write("{0} ", capable_[i][c].jobID);
                        }//capable > 0
                    }//index >= 0 


                }//staff loop

                /*
                //store the possible job for each staff
                List<assign>[] swap = new List<assign>[staffset.Count()];
                // swapping
                for (int i = 0; i < staffset.Count(); i++)
                {
                    int score = initial[i].Last().totalwork - initial[i].Last().totaldrive;
                    //swap the first
                    string s = staffset[i].personID.ToString();
                    List<traveltime> find = GetDrivingTime(trvtime, $"T-{s}", capable_[i][0].jobID.ToString());
                    //doing check

                    
                    
                    swap[i].Add(new assign
                        
                        {
                            job = capable_[i][0],
                            jobID = capable_[i][0].jobID,
                            drvTime = find[0].duration,
                            drv_str = JobDrv[0].drvstr,
                            job_str = JobDrv[0].workstr,
                            job_end = end,
                            drv_back = JobDrv[0].drvback,
                            totalwork = initial[i][num - 1].totalwork + JobDrv[0].job.duration,
                            totaldrive = initial[i][num - 1].totaldrive + JobDrv[0].drvTime,
                        });
                     

                }
                */
                    Console.WriteLine("\n\n{0}", horizon[d]);
                    for (int i = 0; i < staffset.Count(); i++)
                    { 
                        Console.Write("staff {0} assign: ", staffset[i].personID);
                        for (int j = 0; j < initial[i].Count(); j++)
                            Console.Write(" {0}", initial[i][j].jobID);
                        Console.WriteLine();
                    }
                    Console.WriteLine("\n");

                bool duplicate = assignedjob.GroupBy(n => n).Any(c => c.Count() > 1);
                double rate = (double)assignedjob.Count()/ jobset.Count() *100;
                var assignrate = string.Format("{0:0.0}", rate);
                

                Console.WriteLine("\n===================Results===================\nTotal assigned jobs after {0} day ", d+1);
                foreach (Jobs job in assignedjob)
                {
                    Console.Write(" {0}", job.jobID);                    
                }
                if (!duplicate) Console.WriteLine("\n Check completed:Each job is only assigned once.");
                Console.WriteLine("\nTotal staffs:{0}", staffset.Count());
                Console.WriteLine("Total assigned jobs:{0} /{1}. Assigning rate:{2}%", assignedjob.Count(), jobset.Count(), assignrate);


            
            }//date loop

            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds/1000} seconds");

           

        }
    }
}
