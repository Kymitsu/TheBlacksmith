using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith
{
    public abstract class PagedMessage<T>
    {
        public int CurrentPage { get; set; }
        public Dictionary<int, Func<T>> Pages { get; set; }
        public List<ulong> MessageIds { get; set; }

        public PagedMessage()
        {
            CurrentPage = 1;
            Pages = new Dictionary<int, Func<T>>();
            MessageIds = new List<ulong>();
        }

        public abstract void BuildPages();

        public T GetCurrentPage()
        {
            return Pages[CurrentPage].Invoke();
        }
        public T GetNextPage()
        {
            if (CurrentPage < Pages.Count)
                CurrentPage++;

            return Pages[CurrentPage].Invoke();
        }

        public T GetPreviousPage()
        {
            if (CurrentPage != 1)
                CurrentPage--;

            return Pages[CurrentPage].Invoke();
        }
    }
}
