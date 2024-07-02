using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using TicketSystem.Models;

namespace OldMerger
{
    internal class Program
    {
        private static void Load(MySqlConnection sourceConnection)
        {
            Database database = new Database();
            { // companies
                Console.WriteLine("Merging companies");
                MySqlCommand oldCommand = new MySqlCommand("SELECT * FROM new_hdesk.company;", sourceConnection);
                using (var reader = oldCommand.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id") && m.Entity == "company"))
                        {
                            var entity = new Company() { Name = reader.GetString("name"), ShortName = reader.GetString("name") };
                            database.Companies.Add(entity);

                            database.SaveChanges();

                            database.Merge.Add(new MergeEntity() { NewId = entity.Id, OldId = reader.GetInt64("id"), Entity = "company" });
                            Console.WriteLine($"Merged company\n{entity}");
                        }
                    }
                }
                database.SaveChanges();
            }
            {//statuses
                Console.WriteLine("Merging statuses");
                MySqlCommand oldCommand = new MySqlCommand("SELECT * FROM new_hdesk.status WHERE `delete`=0;", sourceConnection);
                using (var reader = oldCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id") && m.Entity == "status"))
                        {
                            var entity = new Status() { Name = reader.GetString("status") };
                            database.Statuses.Add(entity);

                            database.SaveChanges();

                            database.Merge.Add(new MergeEntity() { NewId = entity.Id, OldId = reader.GetInt64("id"), Entity = "status" });
                            Console.WriteLine($"Merged status\n{entity}");
                        }
                    }
                }
                database.SaveChanges();
            }
            {//topics
                Console.WriteLine("Merging topics");
                MySqlCommand oldCommand = new MySqlCommand("SELECT * FROM new_hdesk.subject WHERE `delete`=0;", sourceConnection);
                using (var reader = oldCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id") && m.Entity == "topic"))
                        {
                            var entity = new Topic() { Name = reader.GetString("name") };
                            database.Topics.Add(entity);

                            database.SaveChanges();

                            database.Merge.Add(new MergeEntity() { NewId = entity.Id, OldId = reader.GetInt64("id"), Entity = "topic" });
                            Console.WriteLine($"Merged topic\n{entity}");
                        }
                    }
                }

                database.SaveChanges();
            }
            {//users
                Console.WriteLine("Merging users");
                MySqlCommand oldCommand = new MySqlCommand("SELECT * FROM new_hdesk.users WHERE `delete`=0;", sourceConnection);
                using (var reader = oldCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id") && m.Entity == "user"))
                        {
                            var email = reader.IsDBNull("email") ? "" : reader.GetString("email");
                            var computer = reader.IsDBNull("computer") ? "" : reader.GetString("computer");
                            var phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone");
                            long? telegram = reader.IsDBNull("telegram") ? null : reader.GetInt64("telegram");
                            var oldCompanyId = reader.IsDBNull("company_id") ? 0 : reader.GetInt64("company_id");
                            long? companyId = database.Merge.FirstOrDefault(x => x.OldId == oldCompanyId && x.Entity == "company")?.NewId;

                            if (companyId == null)
                                companyId = 1;


                            var entity = new User()
                            {
                                FullName = reader.GetString("name"),
                                AccessGroupId = 1,
                                CanLogin = true,
                                CompanyId = (long)companyId,
                                Email = email,
                                Name = reader.GetString("user_login").Split('\\').Last(),
                                PCName = computer,
                                PhoneNumber = phone,
                                Telegram = telegram,
                                PasswordHash = Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes("1A2345678b9"))),
                            };

                            if (entity.Name == "Guest" || entity.Name == "dummy")
                            {
                                entity.Name = Transliterator.CyrilicToLatin(entity.FullName);
                            }

                            database.Users.Add(entity);

                            database.SaveChanges();

                            database.Merge.Add(new MergeEntity() { NewId = entity.Id, OldId = reader.GetInt64("id"), Entity = "user" });
                            Console.WriteLine($"Merged user\n{entity}");
                        }
                    }
                }

                database.SaveChanges();
            }
            {//tickets
                Console.WriteLine("Merging tickets");
                MySqlCommand oldCommand = new MySqlCommand("SELECT * FROM new_hdesk.messages WHERE `delete`=0 AND `status` != 3;", sourceConnection);
                using (var reader = oldCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id") && m.Entity == "ticket"))
                        {
                            var oldUserId = reader.GetInt64("user_id");
                            var oldSenderId = reader.GetInt64("user_id");
                            long userId = 0;
                            long senderId = 0;
                            long executorId = 0;
                            try
                            {
                                userId = database.Merge.First(x => oldUserId == x.OldId & x.Entity == "user").NewId;
                                senderId = database.Merge.First(x => oldSenderId == x.OldId & x.Entity == "user").NewId;
                                executorId = database.Merge.First(x => reader.GetInt64("admin_id") == x.OldId & x.Entity == "user").NewId;
                            }
                            catch // message from deleted user
                            {
                                continue;
                            }
                            var topicId = database.Merge.First(x => reader.GetInt64("subject") == x.OldId & x.Entity == "topic").NewId;
                            var status = database.Merge.First(x => reader.GetInt64("status") == x.OldId & x.Entity == "status").NewId;
                            var urgency = reader.GetInt32("priority") - 1;
                            var text = reader.GetString("message");
                            var date = reader.GetDateTime("time_start");
                            DateTime deadlineTime;
                            var finished = false;
                            var finishStatus = 1;

                            if (reader.GetInt64("user_id") == 4997)
                            {
                                if (text.Contains("Новый пользователь"))
                                {
                                    continue;
                                }
                            }

                            if (reader.IsDBNull("deadline"))
                            {
                                deadlineTime = reader.GetDateTime("time_over");
                                finished = true;

                                if (reader.GetString("category") == "incident")
                                {
                                    finishStatus = 0;
                                }
                            }
                            else
                            {
                                deadlineTime = reader.GetDateTime("deadline");
                            }

                            var entity = new Ticket()
                            {
                                UserId = userId,
                                SenderId = senderId,
                                TopicId = topicId,
                                StatusId = status,
                                UrgencyInt = urgency,
                                Text = text,
                                TypeInt = 1,
                                Date = date,
                                DeadlineTime = deadlineTime,
                                Finished = finished,
                                FinishStatusInt = finishStatus,
                                ExecutorId = executorId
                            };

                            database.Tickets.Add(entity);

                            database.SaveChanges();

                            database.Merge.Add(new MergeEntity() { NewId = entity.Id, OldId = reader.GetInt64("id"), Entity = "ticket" });
                            Console.WriteLine($"Merged ticket\n{entity}");
                        }
                    }
                }

                database.SaveChanges();
            }
            
            {//comments
                Console.WriteLine("Merging comments");

                List<Ticket> tickets = database.Tickets.ToList();
                tickets.ForEach(t =>
                {
                    foreach (var ticket in tickets)
                    {
                        MySqlCommand oldCommand = new MySqlCommand("SELECT `id`, `message_id`, `user_id`, `comment`, `time_comment`, `visible` FROM new_hdesk.message_comment WHERE `message_id`=@mid;", sourceConnection);
                        try
                        {
                            oldCommand.Parameters.AddWithValue("@mid", database.Merge.First(x => x.NewId == ticket.Id && x.Entity == "ticket").OldId);
                        }
                        catch { Console.WriteLine("Comment from unmerged user"); continue; }

                        using (var reader = oldCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id") && m.Entity == "comment"))
                                {
                                    try
                                    {
                                        var userId = database.Merge.First(m => m.OldId == reader.GetInt64("user_id") && m.Entity == "user").NewId;
                                        var ticketId = database.Merge.First(m => m.OldId == reader.GetInt64("message_id") && m.Entity == "ticket").NewId;
                                        var entity = new Comment()
                                        {
                                            UserId = userId,
                                            TicketId = ticketId,
                                            CommentTypeInt = reader.GetInt32("visible"),
                                            Date = reader.GetDateTime("time_comment"),
                                            Text = reader.GetString("comment"),
                                        };

                                        database.Comments.Add(entity);

                                        database.SaveChanges();

                                        database.Merge.Add(new MergeEntity() { NewId = entity.Id, OldId = reader.GetInt64("id"), Entity = "comment" });
                                        Console.WriteLine($"Merged comment\n{entity}");
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Comment from deleted user");
                                    }
                                }
                            }
                        }

                        database.SaveChanges();
                    }
                });
            }
            { // usergroups
                Console.WriteLine("Merging usergroups");
                MySqlCommand oldCommand = new MySqlCommand("SELECT * FROM new_hdesk.user_group", sourceConnection);
                using (var reader = oldCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id") && m.Entity == "usergroup"))
                        {
                            var entity = new UserGroup() { Name = reader.GetString("name") };
                            database.UserGroups.Add(entity);

                            database.SaveChanges();

                            database.Merge.Add(new MergeEntity() { NewId = entity.Id, OldId = reader.GetInt64("id"), Entity = "usergroup" });
                            Console.WriteLine($"Merged usergroup\n{entity}");
                        }
                    }
                }

                database.SaveChanges();
            }
            /*{ // user in groups
                Console.WriteLine("Merging user in groups");
                MySqlCommand oldCommand = new MySqlCommand("SELECT * FROM new_hdesk.users_in_group", sourceConnection);
                using (var reader = oldCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!database.Merge.Any(m => m.OldId == reader.GetInt64("id_user") && m.Entity == "useringroup"))
                        {
                        }
                    }
                }

                database.SaveChanges();
            }*/
        }

        private static void Main(string[] args)
        {
            string MySQLConnectionString = "server=cl-srv-hdsk.cl.local;user=xrouu;database=new_hdesk;password=1474545mimosH!;";

            using (MySqlConnection oldConnection = new MySqlConnection(MySQLConnectionString))
            {
                oldConnection.Open();
                Load(oldConnection);
                oldConnection.Close();
            }
        }
    }
}
