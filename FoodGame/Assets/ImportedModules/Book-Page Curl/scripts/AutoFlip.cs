﻿using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Book))]
public class AutoFlip : MonoBehaviour {
    public FlipMode Mode;
    public float PageFlipTime = 1;
    public float TimeBetweenPages = 1;
    public float DelayBeforeStarting = 0;
    public bool AutoStartFlip=true;
    public Book ControledBook;
    public int AnimationFramesCount = 40;
    bool isFlipping = false;
    // Use this for initialization
    void Start () {
        if (!ControledBook)
            ControledBook = GetComponent<Book>();
        if (AutoStartFlip)
            StartFlipping(ControledBook.TotalPageCount);
        ControledBook.OnFlip.AddListener(new UnityEngine.Events.UnityAction(PageFlipped));
	}
    void PageFlipped()
    {
        isFlipping = false;
    }
	public void StartFlipping(int destinationPage)
    {
        StartCoroutine(FlipTo(destinationPage));
    }
    public void FlipRightPage(bool fast = false)
    {
        if (isFlipping) return;
        if (ControledBook.currentPage >= ControledBook.TotalPageCount-1) return;
        isFlipping = true;
        float frameTime = PageFlipTime / AnimationFramesCount;

        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl)*2 / AnimationFramesCount;
        if(fast == true){
            Debug.Log("slow = " + frameTime);
            frameTime = frameTime / 5;
            dx = dx * 5;
            Debug.Log("fast = " + frameTime);
        }
        StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx, fast));
    }
    public void FlipLeftPage(bool fast = false)
    {
        if (isFlipping) return;
        if (ControledBook.currentPage <= 0) return;
        isFlipping = true;
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / AnimationFramesCount;
        if(fast == true){
            Debug.Log("slow = " + frameTime);
            frameTime = frameTime / 5;
            dx = dx * 5;
            Debug.Log("fast = " + frameTime);
        }
        StartCoroutine(FlipLTR(xc, xl, h, frameTime, dx, fast));
    }

    IEnumerator FlipTo(int destinationPage)
    {
        /*
        yield return new WaitForSecondsRealtime(DelayBeforeStarting);
        float frameTime = PageFlipTime / AnimationFramesCount;
        float xc = (ControledBook.EndBottomRight.x + ControledBook.EndBottomLeft.x) / 2;
        float xl = ((ControledBook.EndBottomRight.x - ControledBook.EndBottomLeft.x) / 2)*0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(ControledBook.EndBottomRight.y)*0.9f;
        //y=-(h/(xl)^2)*(x-xc)^2          
        //               y         
        //               |          
        //               |          
        //               |          
        //_______________|_________________x         
        //              o|o             |
        //           o   |   o          |
        //         o     |     o        | h
        //        o      |      o       |
        //       o------xc-------o      -
        //               |<--xl-->
        //               |
        //               |
        float dx = (xl)*2 / AnimationFramesCount;
        */

        
        if(ControledBook.currentPage < destinationPage){
            while (ControledBook.currentPage < destinationPage - 1)
            {
                //StartCoroutine(FlipRTL(xc, xl, h, frameTime, dx));
                FlipRightPage(true);
                yield return new WaitForSecondsRealtime(TimeBetweenPages);
            }
        }
        else
        {                
            while (ControledBook.currentPage > destinationPage + 1)
            {
                FlipLeftPage(true);
                yield return new WaitForSecondsRealtime(TimeBetweenPages);
            }
        }     
        

                
        
    }
    IEnumerator FlipRTL(float xc, float xl, float h, float frameTime, float dx, bool fast = false)
    {
        int frameCount = AnimationFramesCount;
        if(fast == true)
        {
            frameCount = frameCount/5;
        }
        float x = xc + xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);

        ControledBook.DragRightPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < frameCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookRTLToPoint(new Vector3(x, y, 0));
            yield return new WaitForSecondsRealtime(frameTime);
            x -= dx;
        }
        ControledBook.ReleasePage();
    }

    IEnumerator FlipLTR(float xc, float xl, float h, float frameTime, float dx, bool fast = false)
    {
        int frameCount = AnimationFramesCount;
        if(fast == true)
        {
            frameCount = frameCount/5;
        }
        float x = xc - xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
        ControledBook.DragLeftPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < frameCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            ControledBook.UpdateBookLTRToPoint(new Vector3(x, y, 0));
            yield return new WaitForSecondsRealtime(frameTime);
            x += dx;
        }
        ControledBook.ReleasePage();
    }
}
