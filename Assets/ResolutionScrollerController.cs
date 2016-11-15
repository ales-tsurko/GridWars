using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

public class ResolutionScrollerController : MonoBehaviour, IEnhancedScrollerDelegate {

    private List<Resolution> data;
	
    public EnhancedScroller scroller;

    public ResolutionButton prefab;

    public int GetNumberOfCells(EnhancedScroller scroller) {
        return data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
        return 100f;
    }

    public void Display (List<Resolution> _data){
        prefab = Resources.Load<ResolutionButton>("Options/ResolutionButton");
        data = new List<Resolution>();
        data = _data;
        scroller.Delegate = this;
        scroller.ReloadData();
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
        ResolutionButton cellView = scroller.GetCellView(prefab) as ResolutionButton;
        cellView.SetData(data[dataIndex]);
        return cellView;
    }
}
