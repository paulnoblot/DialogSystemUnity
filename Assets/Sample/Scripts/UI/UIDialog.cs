using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : MonoBehaviour
{
    public static UIDialog singleton;
    
    DialogBoard board;

    [SerializeField] GameObject panel = null;
    [SerializeField] Text textName = null;
    [SerializeField] Text textDialog = null;

    [SerializeField] GameObject answerTemplate = null;
    [SerializeField] Transform answerTransform = null;

    public void Start()
    {
        if (singleton == null)
            singleton = this;
    }

    public void ShowWindow(Npc npc, DialogBoard board)
    {
        this.board = board;

        textName.text = npc.NpcName;

        SetNode(board.GetNode(board.rootNode) as NpcNode);

        panel.SetActive(true);
    }

    public void HideWindow()
    {
        (board.state as NpcState).isFirstMeet = false;
        panel.SetActive(false);
    }

    public void SetNode(NpcNode node)
    {
        ClearAnswers();

        textDialog.text = node.content;

        GameObject answer;

        List<Node> nextNodes = board.GetNextNodes(node);
        if (nextNodes.Count > 0)
        {
            foreach (NpcNode n in nextNodes)
            {
                answer = Instantiate(answerTemplate, answerTransform);
                answer.GetComponentInChildren<Text>().text = n.choiceString;
                answer.GetComponent<Button>().onClick.AddListener(() => { SetNode(n); });
                answer.SetActive(true);
            }
        }
        else
        {
            answer = Instantiate(answerTemplate, answerTransform);
            answer.GetComponentInChildren<Text>().text = "Leave";
            answer.GetComponent<Button>().onClick.AddListener(() => { HideWindow(); });
            answer.SetActive(true);
        }
    }

    public void ClearAnswers()
    {
        foreach (Transform child in answerTransform)
        {
            Destroy(child.gameObject);
        }
    }
}
