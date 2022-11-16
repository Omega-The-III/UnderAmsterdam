using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class CubeInteraction : NetworkBehaviour
{
    [SerializeField] private PlayerData playerData;

    private enum Direction { Right, Left, Behind, Front, Up, Down };

    [SerializeField] private Transform PipePreview, PipeHolder;
    [SerializeField] private GameObject connectorPart;
    [SerializeField] private GameObject connectorPartPreview;
    [SerializeField] public NetworkObject[] neighbors;
    private PipeColouring pColouring;

    // When TileOccupied changes value, run OnPipeChanged function
    [Networked(OnChanged = nameof(OnPipeChanged))]
    public bool TileOccupied { get; set; } // can be changed and send over the network only by the host

    // When company's values changes, run OnCompanyChange
    [SerializeField]
    [Networked(OnChanged = nameof(onCompanyChange))]
    public string company { get; set; }

    private GameObject[] pipeParts;
    private GameObject[] previewPipeParts;
    private bool[] activatedPipes;

    private int amountFaces = 6;
    private bool isSpawned = false;

    public bool isHover = false;
    public bool isChecked = false;

    void Start()
    {
        pColouring = GetComponent<PipeColouring>();
    }
    public override void Spawned()
    {
        // Hides the tiles
        OnRenderPipePreview(false);
        OnRenderPipePart(false);

        neighbors = new NetworkObject[amountFaces]; //Cubes have 6 faces, thus we will always need 6 neigbors
        GetNeighbors();

        pipeParts = new GameObject[neighbors.Length];
        previewPipeParts = new GameObject[neighbors.Length];

        int i = 0;
        foreach (Transform pipe in PipeHolder)
            pipeParts[i++] = pipe.gameObject;
        i = 0;
        foreach (Transform pipePreview in PipePreview)
            previewPipeParts[i++] = pipePreview.gameObject;

        activatedPipes = new bool[neighbors.Length]; //Array of booleans storing which orientation is enabled [N, S, E, W, T, B]

        TileOccupied = false;
        isSpawned = true;
    }

    // Gets the neighbors tiles in all 6 directions
    private void GetNeighbors()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up, out hit))
            neighbors[(int)Direction.Up] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Up] = null;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            neighbors[(int)Direction.Down] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Down] = null;

        if (Physics.Raycast(transform.position, Vector3.left, out hit))
            neighbors[(int)Direction.Left] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Left] = null;

        if (Physics.Raycast(transform.position, Vector3.right, out hit))
            neighbors[(int)Direction.Right] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Right] = null;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
            neighbors[(int)Direction.Front] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Front] = null;

        if (Physics.Raycast(transform.position, Vector3.back, out hit))
            neighbors[(int)Direction.Behind] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Behind] = null;
    }

    public void checkWin()
    {
        // For each neighbor...
        for (int i = 0; i < neighbors.Length; i++)
        {
            // if it's a normal tile...
            if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
            {
                // from the same company and not checked yet...
                if (company == neighborTile.company && !neighborTile.isChecked)
                {
                    // Verify its neighbor and mark it as checked.
                    isChecked = true;
                    neighborTile.checkWin();
                }
            }
            // if it's an Output tile...
            else if (neighbors[i].TryGetComponent(out IOTileData inOutPut))
            {
                // from the same company and active...
                if (company == inOutPut.company && inOutPut.isActive)
                {
                    inOutPut.winGameEvent(); // Success !
                    return;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSpawned && TileOccupied && other.CompareTag("HammerHead"))
            DisableTile();
    }

    public void OnHandEnter(string playerCompany)
    {
        if (isSpawned && !TileOccupied)
        {
            isHover = true;
            UpdateNeighborData(true, playerCompany);
            OnRenderPipePreview(true);
        }
    }

    public void OnHandExit(string playerCompany)
    {
        if (isSpawned && !TileOccupied)
        {
            isHover = false;
            OnRenderPipePreview(false);
            UpdateNeighborData(false, playerCompany);
        }
    }
    static void onCompanyChange(Changed<CubeInteraction> changed)
    {
        // When company changes give the new company (changed is the new values)
        changed.Behaviour.UpdateCompany(changed.Behaviour.company);
        changed.Behaviour.UpdateNeighborData(true);
    }

    // Check all of its neighbors and activate the corresponding pipes if it's from the same company
    private void UpdateNeighborData(bool enable, string playerCompany = ".")
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                // Gets the neighbor tile
                if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                {
                    // If the neighbor is in the same company...
                    if (neighborTile.company != "Empty" && (neighborTile.company == company || neighborTile.company == playerCompany))
                    {
                        // activates the pipe facing the neighbor as well as the neighbor's pipe facing the current tile.
                        activatedPipes[i] = enable;
                        neighborTile.activatedPipes[GetOppositeFace(i)] = enable;
                    }
                }
                // Or the IO tile
                else if (neighbors[i].TryGetComponent(out IOTileData IOTile))
                {
                    if (IOTile.company != "Empty" && (IOTile.company == company || IOTile.company == playerCompany))
                        activatedPipes[i] = enable;
                }
            }
        }
    }
    [Tooltip("Should be activated before EnableTile()")]
    public void UpdateCompany(string newCompany)
    {
        company = newCompany;
        pColouring.UpdateRenderer(company);
    }

    // Enable/Disable Tile
    public void EnableTile()
    {
        playerData.points -= 20;

        isHover = false;
        TileOccupied = true;
        OnRenderPipePart(true);
        UpdateNeighborData(true);
        pColouring.UpdateRenderer(company);
        OnRenderPipePreview(false);
    }
    private void DisableTile()
    {
        playerData.points += 20;

        company = "Empty";
        TileOccupied = false;
        // Deactivate all pipes
        OnRenderPipePart(false);
        OnRenderPipePreview(false);
        for (int i = 0; i < neighbors.Length; i++)
        {
            activatedPipes[i] = false;
            if (neighbors[i] != null && neighbors[i].TryGetComponent(out CubeInteraction neighborTile)) neighborTile.activatedPipes[GetOppositeFace(i)] = false;
        }

    }

    private void OnRenderPipePart(bool isActive)
    {
        connectorPart.SetActive(isActive);
        pColouring.UpdateRenderer(company, connectorPart);

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (pipeParts[i] != null && activatedPipes[i])
            {
                //Display/undisplay every pipe which is activated
                pipeParts[i].SetActive(isActive);

                if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                {
                    if (neighborTile.activatedPipes[GetOppositeFace(i)])
                    {
                        neighborTile.pipeParts[GetOppositeFace(i)].SetActive(isActive);
                        neighborTile.pColouring.UpdateRenderer(company);
                    }
                }
            }
        }
    }

    private void OnRenderPipePreview(bool isActive)
    {
        connectorPartPreview.SetActive(isActive);
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (previewPipeParts[i] != null && activatedPipes[i])
            {
                //Display/undisplay every pipe which is activated
                previewPipeParts[i].SetActive(isActive);

                if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                {
                    if (neighborTile.activatedPipes[GetOppositeFace(i)])
                        neighborTile.previewPipeParts[GetOppositeFace(i)].SetActive(isActive);
                }
            }
        }
    }

    // This code gets ran ON OTHER PLAYERS when a pipe has been placed, changed is the new values of the placed pipe
    static void OnPipeChanged(Changed<CubeInteraction> changed) // static because of networked var isPiped
    {
        bool isPipedCurrent = changed.Behaviour.TileOccupied;

        changed.Behaviour.OnPipeRender(isPipedCurrent);
    }

    // Run this code locally for players where pipe hasn't changed yet
    void OnPipeRender(bool isPipedCurrent)
    {
        if (isPipedCurrent)
            EnableTile();
    }
    private int GetOppositeFace(int i)
    {
        // Getting the index of the opposite direction to i
        return i + 1 - 2 * (i % 2);
    }
}
