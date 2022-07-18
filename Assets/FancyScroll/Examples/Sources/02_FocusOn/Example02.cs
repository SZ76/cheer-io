/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example02
{
    class Example02 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        public Tile tile;

        void Start()
        {
            scrollView = this.transform.Find("Hotbar").Find("Scroll(Clone)").Find("ScrollView").GetComponent<ScrollView>();
            tile = GameObject.FindGameObjectWithTag("TileManager").GetComponent<Tile>();
            scrollView.OnSelectionChanged(OnSelectionChanged);

            var items = Enumerable.Range(Tile.startScrollNum, Tile.scrollNum - Tile.startScrollNum)
                .Select(i => new ItemData(i + ""))
                .ToArray();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);
        }

        void OnSelectionChanged(int index)
        {
            tile.reduce = (byte)(index + Tile.startScrollNum);
        }
    }
}
