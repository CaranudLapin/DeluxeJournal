﻿using Newtonsoft.Json;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Framework;

using static DeluxeJournal.Task.ITask;

namespace DeluxeJournal.Task
{
    /// <summary>Task base class. All tasks should derive from this.</summary>
    public abstract class TaskBase : ITask
    {
        private readonly string _id;

        protected bool _complete;
        protected bool _viewed;
        private int _index;

        public string ID => _id;

        public string Name { get; set; }

        public long OwnerUMID { get; set; }

        public Period RenewPeriod { get; set; }

        public WorldDate RenewDate { get; set; }

        public virtual int Count { get; set; }

        public virtual int MaxCount { get; set; }

        public virtual int BasePrice { get; set; }

        public virtual bool Active { get; set; }

        public virtual bool Complete
        {
            get
            {
                return _complete;
            }

            set
            {
                _complete = value;
                _viewed = !value;

                if (!value && Count >= MaxCount)
                {
                    Count = 0;
                }
            }
        }

        protected TaskBase(string id) : this(id, string.Empty)
        {
        }

        protected TaskBase(string id, string name)
        {
            _id = id;
            _complete = false;
            _viewed = true;
            _index = 0;

            Name = name;
            Active = true;
            OwnerUMID = Game1.player?.UniqueMultiplayerID ?? 0;
            RenewPeriod = Period.Never;
            RenewDate = new WorldDate(1, "spring", 1);
            Count = 0;
            MaxCount = 1;
            BasePrice = 0;
        }

        private static int TotalDaysInYear(WorldDate date)
        {
            return date.SeasonIndex * 28 + date.DayOfMonth;
        }

        public virtual ITask Copy()
        {
            ITask copy = (ITask)MemberwiseClone();
            copy.RenewDate = new WorldDate(copy.RenewDate);
            return copy;
        }

        /// <summary>Is this task ready to receive update events?</summary>
        public bool CanUpdate()
        {
            return Active && !Complete;
        }

        /// <summary>Mark as completed and trigger audio/visual cues.</summary>
        public virtual void MarkAsCompleted()
        {
            if (!Complete)
            {
                Complete = true;
                Game1.playSound("jingle1");

                if (DeluxeJournalMod.Instance?.Config is Config config && config.EnableVisualTaskCompleteIndicator)
                {
                    Game1.dayTimeMoneyBox.PingQuestLog();
                }
            }
        }

        /// <summary>Increment progress count and optionally mark as completed if <c>MaxCount</c> is met.</summary>
        /// <param name="amount">Amount to increment <c>Count</c> by.</param>
        /// <param name="markAsCompleted">Mark as completed if <c>Count >= MaxCount</c>.</param>
        protected void IncrementCount(int amount = 1, bool markAsCompleted = true)
        {
            Count += amount;

            if (Count >= MaxCount)
            {
                Count = MaxCount;

                if (markAsCompleted)
                {
                    MarkAsCompleted();
                }
            }
        }

        public virtual int DaysRemaining()
        {
            return RenewPeriod switch
            {
                Period.Weekly => (((RenewDate.DayOfMonth - Game1.dayOfMonth) % 7) + 7) % 7,
                Period.Monthly => (((RenewDate.DayOfMonth - Game1.dayOfMonth) % 28) + 28) % 28,
                Period.Annually => (((TotalDaysInYear(RenewDate) - TotalDaysInYear(Game1.Date)) % 112) + 112) % 112,
                _ => 0,
            };
        }

        public virtual int GetPrice()
        {
            return BasePrice * (MaxCount - Count);
        }

        public int GetSortingIndex()
        {
            return _index;
        }

        public void SetSortingIndex(int index)
        {
            _index = index;
        }

        public virtual void MarkAsViewed()
        {
            _viewed = true;
        }

        public virtual bool HasBeenViewed()
        {
            return _viewed;
        }

        public virtual bool ShouldShowProgress()
        {
            return false;
        }

        public virtual bool ShouldShowCustomStatus()
        {
            return false;
        }

        public virtual string GetCustomStatusKey()
        {
            return string.Empty;
        }

        public bool IsTaskOwner(Farmer farmer)
        {
            return OwnerUMID == farmer.UniqueMultiplayerID;
        }

        public bool IsTaskOwner(long umid)
        {
            return OwnerUMID == umid;
        }

        public virtual void EventSubscribe(ITaskEvents events)
        {
        }

        public virtual void EventUnsubscribe(ITaskEvents events)
        {
        }

        public virtual void Validate()
        {
        }

        public int CompareTo(ITask? other)
        {
            if (other == null)
            {
                return 1;
            }
            else if (Active && other.Active)
            {
                return (Complete == other.Complete) ? _index - other.GetSortingIndex() : Complete.CompareTo(other.Complete);
            }
            else if (!Active && !other.Active)
            {
                return DaysRemaining() - other.DaysRemaining();
            }
            else
            {
                return other.Active.CompareTo(Active);
            }
        }
    }
}
