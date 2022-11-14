using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeColouring : MonoBehaviour
{
    [Tooltip("Circle between connecting pipes")]
    [SerializeField] private GameObject connectorPiece;
    [Tooltip("Parent of where all pipes are stored")]
    [SerializeField] private GameObject pipeParent;

    [Tooltip("All pipe materials, named same as company")]
    [SerializeField] private Material[] pipeMaterials;
    [Tooltip("All pipe options")]
    [SerializeField] private GameObject[] pipeChildren;

    public void UpdateRenderer(string pipeCompany, GameObject givenGO = null) {
        if(!givenGO) {
            for (int i = 0; i < pipeChildren.Length; i++)
            {
                 // Go through the children and find the active one
                if(pipeChildren[i].activeSelf == true)
                {
                    // Give the company and the gameobject where the materials are on
                    ColourPipe(pipeCompany, pipeChildren[i].transform.GetChild(0).gameObject);
                }
            }
        } else {
            // Is anything other than a pipe, like connector piece
            ColourPipe(pipeCompany, givenGO);
        }
    }

    // Colour the connecting pipes when placed
    void ColourPipe(string company, GameObject affectedChild) {
        
        for (int i = 0; i < pipeMaterials.Length; i++)
        {
            if (pipeMaterials[i].name == company) {
                affectedChild.GetComponent<Renderer>().material = pipeMaterials[i];
                }
            }
        }
}
