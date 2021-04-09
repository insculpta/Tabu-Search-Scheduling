using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TeampointScheduling;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TeampointScheduling
{
    public class Person
    {
        public int personID;
        public int max_minutes;
        public int max_miles;
        public int[] p_tags; //qualification tags
        public int[] dayoff;
        public int day_start; //initial position ID

     
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

    }

    public class DaySchedule 
    {
        public int personID;
        public int[] p_tags;
        public List<Job> duty;
        public int dutyTime; // total working time
        public int drivingTime; // total driving distance       
        public int num_duty; //total work duty
        public int end_time; //current all the duty end time
         
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
            if (File.Exists(@"testoperative.csv"))  //判斷檔案是否存在
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
            int work_begin= 25200 ; // the time operators go to work 
            
            //List<int> jobtag = new List<int>(); 
            //List<int> operativetag = new List<int>(); 
            //List<int> personID = new List<int>(); 
            // var person1 = new person();  


            //int[] max_minutes= { 540, 540, 720, 322 ,480 , 540, 420,240,540,540, 540, 540, 720, 322, 480, 540, 420, 240, 540, 540, };
            // int[] max_miles = { 402, 402, 322, 402, 0, 402, 402, 402, 467, 0, 322, 402, 402, 563, 322, 483, 483, 402, 322, 161 };
            
            //operative data
            List<int> personid = new List<int>();
            List<int> max_minutes = new List<int>();
            List<int> max_miles = new List<int>();
            List<string> dayoff = new List<string>();
            List<int> worker_tags = new List<int>();
            // read from excel
            //ReadFile(personID, max_minutes, max_miles, tags, dayoff);


            //read operative from MySQL
            /*string cs = @"Server = 127.0.0.1; Port = 3306; Database = test; Uid = root; Pwd = rex840406";            
            using var con = new MySqlConnection(cs);
            con.Open();

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
            drive.Add(new location() { from = 8, to = 13, duration = 1800 });
            drive.Add(new location() { from = 2, to = 9, duration = 1200 });
            drive.Add(new location() { from = 9, to = 14, duration = 1200 });
            drive.Add(new location() { from = 4, to = 10, duration = 1600 });
            drive.Add(new location() { from = 10, to = 15, duration = 1600 });
            drive.Add(new location() { from = 5, to = 11, duration = 1800 });
            drive.Add(new location() { from = 11, to = 16, duration = 1800 });
            drive.Add(new location() { from = 6, to = 12, duration = 1200 });
            drive.Add(new location() { from = 12, to = 17, duration = 1200 });
            drive.Add(new location() { from = 0, to = 1, duration = 1200 });
            drive.Add(new location() { from = 0, to = 2, duration = 1200 });
            drive.Add(new location() { from = 0, to = 3, duration = 1200 });
            drive.Add(new location() { from = 0, to = 4, duration = 1200 });
            drive.Add(new location() { from = 0, to = 5, duration = 1200 });
            drive.Add(new location() { from = 0, to = 6, duration = 1200 });
            drive.Add(new location() { from = 0, to = 7, duration = 1200 });
            // drive.Add(new location() { from = 3, to = 1, duration = 1600 });
            //drive.Add(new location() { from = 3, to = 2, duration = 1600 });



            //work data
            List<Job> jobs = new List<Job>();
            jobs.Add(new Job() { jobID = 1, job_dates = 1, win_start = 36000, win_end = 43200, tags = new int[] {    }, duration = 7200});
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
            person.Add(new Person() { personID = 1, p_tags = new int[] { 11 }, dayoff = new int[] {   } , day_start = 0 });
            person.Add(new Person() { personID = 2, p_tags = new int[] { 12 }, dayoff = new int[] { 1 } , day_start = 0 });
            person.Add(new Person() { personID = 3, p_tags = new int[] { 13 }, dayoff = new int[] { 3 } , day_start = 0 });

            List<DaySchedule> test = new List<DaySchedule>();
            test.Add(new DaySchedule() { personID = 1, duty = new List<Job>() });
            test[0].duty.Add(jobs[1]);

            Console.WriteLine("here is {0}", test[0].duty[0].win_start);

            //dayoffjobs.Add(jobs[0]);
            Console.WriteLine("here is test priority {0}", dayoffjobs[3].priority);

            foreach (int x in dayoffjobs[0].tags)
            {
                Console.WriteLine("here is {0}", x);
            }

            //store the possible job for each person
            List<Job>[] capable = new List<Job>[person.Count()];

            //****Tag Constraint:  Match jobs with people ------------------------------------------------------------
            for (int i = 0; i < person.Count(); i++)
            {
                capable[i] = new List<Job>(); // create each person's capable list
                for (int j = 0; j < jobs.Count(); j++)
                {
                    bool except = jobs[j].tags.Except(person[i].p_tags).Any();  //compare two tags array
                    if (!except) //False means a person has all the tags a job needs
                    {
                        capable[i].Add(jobs[j]); //add possible job to that person
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

            //List<DaySchedule>[] sol_day = new List<DaySchedule>[person.Count()];

            //****Tag Constraint:  Match jobs with people ------------------------------------------------------------
            for (int i = 0; i < person.Count(); i++)
            {
                capable[i] = new List<Job>(); // create each person's capable list
                for (int j = 0; j < jobs.Count(); j++)
                {
                    bool except = jobs[j].tags.Except(person[i].p_tags).Any();  //compare two tags array
                    if (!except) //False means a person has all the tags a job needs
                    {
                        capable[i].Add(jobs[j]); //add possible job to that person
                    }
                }
            }

            //store the possible job for each person
            List<Job>[] day1 = new List<Job>[person.Count()];
            List<Job>[] sol_day1 = new List<Job>[person.Count()];
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
                sol_day1[i] = new List<Job>();
                day1[i] = capable[i].FindAll(x => x.job_dates == 1);

                //***** Dayoff Constraints:---------------------------------------------------------------------------------------  
                if (person[i].dayoff.Contains(1)) //if person dayoff on day 1
                {                    
                    day1[i].Clear();
                    day1[i].Add(dayoffjobs.Find(x => x.jobID == 0 && x.job_dates == 1)); // Assign special dayoff job to the person
                }
                
                day1[i] = day1[i].OrderBy(o => o.win_start).ToList();
            }
            //print possible duty
            for (int i = 0; i < person.Count(); i++)
            {
                Console.WriteLine("person{0} possible job in day1:", i + 1);
                for (int j = 0; j < day1[i].Count(); j++)
                    Console.Write(" {0}", day1[i][j].jobID);
                    Console.WriteLine();
            }
            Console.WriteLine("\n");

            //solution
            for (int i = 0; i < person.Count(); i++)
            {
                //***** Only One person for a job Constraint:------------------------------------------------------------------
                // from person 2, remove all jobs in the possible duty that already done by previous people

                if (i > 0 && day1[i].Any() && day1[i][0].jobID != 0) //day[i] is not empty and the only obj is not dayoff   
                {
                    for (int k = 0; k < i; k++)
                    {
                        for (int p = 0; p < day1[i].Count(); p++)
                            Console.WriteLine("before p{0} can do {1}", i+1, day1[i][p].jobID);
                        
                        day1[i] = day1[i].Except(sol_day1[k]).ToList(); //Tolist turns IEnumerable(the return of except) to list  

                        for (int p = 0; p < day1[i].Count(); p++)
                            Console.WriteLine("after except p{2}'s duty, p{0} can do {1}", i+1, day1[i][p].jobID, k+1);
                    }
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
                    Console.WriteLine("Add first duty {0} to p{1}", day1[i][0].jobID, i + 1);
                    // add duties
                    for (int j = 0; j < day1[i].Count() - 1; j++)
                    {
                        //Console.WriteLine("day{0}{1}= {2};day{3}{4}={5}", i,j, day1[i][j].win_end,i,j+1, day1[i][j+1].win_start);


                        //***** Driving Constraints:--------------------------------------------------------------------------------- 

                        int index = sol_day1[i].Count(); // person i-1 's last duty
                        int id = sol_day1[i][index - 1].jobID;
                        int id_next = day1[i][j + 1].jobID;
                        List<location> temp_1 = new List<location>();
                        temp_1.Add(drive.Find(x => x.from == id && x.to == id_next));
                        Console.WriteLine("From job{0} to job{1} duration = {2}", id, id_next, temp_1[0].duration);

                        //make sure the start time of later job is behind the start time of the previous one
                        if ((sol_day1[i][index - 1].win_end + temp_1[0].duration) <= day1[i][j + 1].win_start)
                        {
                            sol_day1[i].Add(day1[i][j + 1]);
                            Console.WriteLine("Add {0} to p{1}", day1[i][j + 1].jobID, i + 1);
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



            //-------d1-----Initial solution end---------------------------------






            //-------d2-----Initial solution ----------------------------------------------



            //store the possible job for each person
            List<Job>[] day2 = new List<Job>[person.Count()];
            List<Job>[] sol_day2 = new List<Job>[person.Count()];
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
                Console.WriteLine("person{0} possible job in day3:", i + 1);
                for (int j = 0; j < day[0][i].Count(); j++)
                    Console.Write(" {0}", day[0][i][j].jobID);
                Console.WriteLine();
            }
            Console.WriteLine("\n");
            */

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
