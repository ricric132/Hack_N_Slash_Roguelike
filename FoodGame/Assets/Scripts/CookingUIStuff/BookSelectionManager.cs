using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSelectionManager : MonoBehaviour
{
    public Book book;
    public AutoFlip pageFlipper;
    public CookbookItemMatch cookbookItem;
    public Texture placeholderPage;
    public List<PageSpread> pageSpreads;
    public bool isSelected;
    public int  savedPage;
    public CookbookItemSO selectedDish;
    public PlayerStatus playerStatus;
    public GameObject searchPageSelectionBoxes;
    public GameObject cookButton;

    int addedPages = 0;

    [Serializable]
    public class PageSpread{
        public CookbookItemSO item;
        public Texture L;
        public Texture R;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isSelected){
            if(book.currentPage < book.bookPages.Count - 1){
                cookButton.SetActive(false);
                searchPageSelectionBoxes.SetActive(true);
            }
            else{
                cookButton.SetActive(true);
                searchPageSelectionBoxes.SetActive(false);
            }
        }
    }

    public void selectMenu(int placement){
        cookbookItem.updateBook();
        int index = (book.currentPage - 1) * 3 + placement;
        if(index < 0 || index > cookbookItem.displayedDishes.Count - 1){
            return;
        }
        
        InstantDeleteAddedPage();
        searchPageSelectionBoxes.SetActive(false);

        CookbookItemSO clicked = cookbookItem.displayedDishes[index];
        Debug.Log("index = " + index);
        Debug.Log("name = " + clicked.name);

        if (book.TotalPageCount%2 == 0){
            FillExtraPage();
        }

        foreach(PageSpread pageSpread in pageSpreads){
            if(pageSpread.item == clicked){
                book.bookPages.Add(pageSpread.L);
                book.bookPages.Add(pageSpread.R);
                selectedDish = pageSpread.item;
                break;
            }
        }
        addedPages += 2;

        savedPage = book.currentPage;
        isSelected = true;
        pageFlipper.StartFlipping(book.TotalPageCount);

        cookButton.SetActive(true);
        
    }

    public void returnToSearch(){
        cookButton.SetActive(false);

        pageFlipper.StartFlipping(savedPage);
        isSelected = false;
        selectedDish = null;

        searchPageSelectionBoxes.SetActive(true);
        
        StartCoroutine(DeleteAddedPages());
    }

    IEnumerator DeleteAddedPages(){
        yield return new WaitForSeconds(0.3f);
        for(int i = 0; i < addedPages; i ++){
            book.bookPages.RemoveAt(book.TotalPageCount - 1);
        }

        addedPages = 0;
    }

    void InstantDeleteAddedPage(){
        for(int i = 0; i < addedPages; i ++){
            book.bookPages.RemoveAt(book.TotalPageCount - 1);
        }

        addedPages = 0;

    }

    void FillExtraPage(){
        addedPages += 1;
        book.bookPages.Add(placeholderPage);
    }

    public void Cook(){
        if(isSelected == true){
             playerStatus.AddCookedItem(selectedDish); 
        }
    }
}
