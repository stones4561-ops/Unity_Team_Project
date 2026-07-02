using TMPro;
using UnityEngine;

public class SplitPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private InventoryUI invenUI;

    private Slot sourceSlot;
    private Slot targetSlot;

    


    public void OpenPopup(Slot _source,Slot _target)
    {
        sourceSlot = _source;
        targetSlot = _target;

        inputField.text = "";
        gameObject.SetActive(true);
    }

    public void OnConfirmClick()
    {
        int splitAmount = 0;

        if(int.TryParse(inputField.text, out splitAmount))
        {
            if(splitAmount>0&&splitAmount<sourceSlot.CurItemMany)
            {
                sourceSlot.ItemDown(splitAmount);

                targetSlot.SetItem(sourceSlot.CurItemData, splitAmount);

                invenUI.Redraw();

                gameObject.SetActive(false);
            }
            else if(splitAmount>=sourceSlot.CurItemMany)
            {
                Debug.Log("숫자입력이 잘못됐습니다.");
            }
        }

        
    }

    public void OnCancelClick()
    {
        gameObject.SetActive(false);
    }
}
