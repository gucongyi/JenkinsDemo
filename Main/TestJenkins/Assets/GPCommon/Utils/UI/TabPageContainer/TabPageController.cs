using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GPCommon
{
    public interface ITabTitle
    {
        void SetContainer(TabPageController tabPageController);
        GameObject TitleGameObject { get; }
        GameObject TitlePage { get; }
        void SetDepth(int i);
        void OnSelected(bool b);
    }

    public class TabPageController : MonoBehaviour
    {
        private const string SwitchTitleMessage = "OnSwitchTitle";

        public int TopDepth;

        [Tooltip("Public for debugging")] public List<ITabTitle> Titles;
        [Tooltip("Public for debugging")] public ITabTitle SelectedTitle;
        [Tooltip("Public for debugging")] public GameObject DefaultPage;

        private bool _inited;

        void Awake()
        {
            Init();
        }

        public void Init()
        {
            if(_inited)
                return;

            if (Titles == null)
            {
                Titles = transform.GetComponentsInChildren<ITabTitle>(true).ToList();
                Titles.ForEach(t =>
                {
                    t.SetContainer(this);

                    if (t.TitlePage.activeSelf)
                        DefaultPage = t.TitlePage;
                });
            }

            ShowDefaultPage();

            _inited = true;
        }

        public void ShowDefaultPage()
        {
            foreach (var t in Titles)
            {
                if (t.TitlePage == DefaultPage)
                {
                    SelectTitle(t, false);
                    return;
                }
            }
        }

        public void SelectTitle(ITabTitle title, bool sendMessage)
        {
            if (SelectedTitle == title)
                return;

            // Hide current page
            if (SelectedTitle != null && SelectedTitle.TitlePage.activeSelf &&
                title.TitlePage != SelectedTitle.TitlePage)
            {
                SelectedTitle.TitlePage.SetActive(false);
            }

            UpdateTabTitles(title);

            // Show title's page
            if (title.TitlePage != null)
            {
                if (!title.TitlePage.activeSelf)
                    title.TitlePage.SetActive(true);

                if(sendMessage)
                    title.TitlePage.SendMessage(SwitchTitleMessage, title, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void UpdateTabTitles(ITabTitle selectedTitle)
        {
            SelectedTitle = selectedTitle;

            var selectedIndex = Titles.IndexOf(selectedTitle);
            for (var i = 0; i < Titles.Count; i++)
            {
                var t = Titles[i];
                t.SetDepth(TopDepth - Math.Abs(selectedIndex - i));
                t.OnSelected(t == selectedTitle);
            }
        }
    }
}