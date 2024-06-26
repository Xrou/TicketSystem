﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace TicketSystemDesktop.Models
{
    public class Comment : INotifyPropertyChanged, IDbEntity
    {
        private long id;
        private string text;
        private DateTime date;
        private long ticketId;
        private long userId;
        private string userName;
        private string fullName;
        private int commentTypeInt;
        private string[] files;

        public long Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged("Id"); }
        }
        public string Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged("Text"); }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; OnPropertyChanged("Date"); }
        }
        public long TicketId
        {
            get { return ticketId; }
            set { ticketId = value; OnPropertyChanged("TicketId"); }
        }
        public long UserId
        {
            get { return userId; }
            set { userId = value; OnPropertyChanged("UserId"); }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged("UserName"); }
        }
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; OnPropertyChanged("FullName"); }
        }
        public int CommentTypeInt
        {
            get { return commentTypeInt; }
            set { commentTypeInt = value; OnPropertyChanged("CommentTypeInt"); }
        }
        public string[] Files
        {
            get { return files; }
            set { files = value; OnPropertyChanged("Files"); }
        }

        public object MainParameter { get { return text; } }

        public static Comment ParseFromJson(JsonObject parseObject)
        {
            Comment comment = new Comment();

            comment.Id = parseObject["id"]!.GetValue<long>();
            comment.Text = parseObject["text"]!.GetValue<string>();
            comment.Date = parseObject["date"]!.GetValue<DateTime>();
            comment.TicketId = parseObject["ticketId"]!.GetValue<long>();
            comment.UserId = parseObject["userId"]!.GetValue<long>();
            comment.UserName = parseObject["userName"]!.GetValue<string>();
            comment.FullName = parseObject["fullName"]!.GetValue<string>();
            comment.CommentTypeInt = parseObject["type"]!.GetValue<int>();

            var filesList = new List<string>();

            foreach (var file in parseObject["files"].AsArray())
            {
                filesList.Add(file.GetValue<string>());
            }

            comment.Files = filesList.ToArray();

            return comment;
        }

        public static List<Comment> ParseArrayFromJson(string response)
        {
            List<Comment> parsed = new List<Comment>();
            var array = JsonArray.Parse(response);

            foreach (var t in array.AsArray())
            {
                var obj = t.AsObject();
                parsed.Add(ParseFromJson(obj));
            }

            return parsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
