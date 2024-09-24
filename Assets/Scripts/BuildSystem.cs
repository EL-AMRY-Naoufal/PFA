using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.AI;
using System.Collections.Generic;

public class BuildSystem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private Transform placeStructuresParent; 

    [SerializeField]
    private Structure[] structures;

    [SerializeField]
    private Material blueMaterial;

    [SerializeField]
    private Material redMaterial;

    [SerializeField]
    private Transform rotationRef;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip buildingSound;

    [Header("UI References")]
    [SerializeField]
    private Transform buildSystemUIPanel;

    [SerializeField]
    private GameObject buildingRequiredElement;

    private Structure currentStructure;
    private bool inPlace;
    private bool canBuild;
    private Vector3 finalPosition;
    private bool systemEnabled = false;

    public List<PlacedStructure> placedStructures;
    private void Awake()
    {
        currentStructure = structures.First();
        DisableSystem();
    }

    private void FixedUpdate()
    {
        if (!systemEnabled)
            return;

        canBuild = currentStructure.placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().CheckConnection();
        finalPosition = grid.GetNearestPointOnGrid(transform.position);
        CheckPosition();
        RoundPlacamentStructureRotation();
        UpdatePlacementStructureMateriel();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentStructure.structureType == StructureType.Stairs && systemEnabled)
                DisableSystem();
            else
                ChangeStructureType(GetStructureByType(StructureType.Stairs));

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentStructure.structureType == StructureType.Wall && systemEnabled)
                DisableSystem();
            else
                ChangeStructureType(GetStructureByType(StructureType.Wall));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentStructure.structureType == StructureType.Floor && systemEnabled)
                DisableSystem();
            else
                ChangeStructureType(GetStructureByType(StructureType.Floor));
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) && canBuild && inPlace && systemEnabled && HasAllRessources())
        {
            BuildStructure();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            RotateStructure();
        }
    }

    void BuildStructure()
    {
        Instantiate(currentStructure.instantiatedPrefab, currentStructure.placementPrefab.transform.position, currentStructure.placementPrefab.transform.GetChild(0).transform.rotation, placeStructuresParent);

        placedStructures.Add(new PlacedStructure { prefab = currentStructure.instantiatedPrefab, positions = currentStructure.placementPrefab.transform.position, rotations = currentStructure.placementPrefab.transform.GetChild(0).transform.rotation.eulerAngles });

        audioSource.PlayOneShot(buildingSound);

        for (int i = 0; i < currentStructure.ressourcesCost.Length; i++)
        {
            for (int j = 0; j < currentStructure.ressourcesCost[i].count; j++)
            {
                Inventory.instance.RemoveItem(currentStructure.ressourcesCost[i].itemData);
            }
        }
    }

    bool HasAllRessources()
    {
        BuildingRequiredElement[] requiredElements = GameObject.FindObjectsOfType<BuildingRequiredElement>();
        return requiredElements.All(requiredElement => requiredElement.hasResource);
    }

    void DisableSystem()
    {
        systemEnabled = false;

        buildSystemUIPanel.gameObject.SetActive(false); 

        currentStructure.placementPrefab.SetActive(false);
    }

    void RoundPlacamentStructureRotation()
    {
        float Yangle = rotationRef.localEulerAngles.y;
        int roundedRotaion = 0;

        if (Yangle > -45 && Yangle <= 45)
        {
            roundedRotaion = 0;
        }
        else if(Yangle >45 && Yangle<= 135)
        {
            roundedRotaion = 90;
        }
        else if (Yangle > 135 && Yangle <= 225)
        {
            roundedRotaion = 180;
        }
        else if (Yangle > 255 && Yangle <= 315)
        {
            roundedRotaion = 270;
        }

        currentStructure.placementPrefab.transform.rotation = Quaternion.Euler(0, roundedRotaion, 0);
    }

    void RotateStructure()
    {
        if(currentStructure.structureType != StructureType.Wall)
        {
            currentStructure.placementPrefab.transform.GetChild(0).transform.Rotate(0, 90, 0);
        }
    }

    void UpdatePlacementStructureMateriel()
    {
        MeshRenderer placementPrefabRenderer = currentStructure.placementPrefab.GetComponentInChildren<CollisionDetectionEdge>().meshRenderer;

        if(inPlace && canBuild && HasAllRessources())
        {
            placementPrefabRenderer.material = blueMaterial;
        }
        else
        {
            placementPrefabRenderer.material = redMaterial;
        }
    }

    void CheckPosition()
    {
        inPlace = currentStructure.placementPrefab.transform.position == finalPosition;
        if(!inPlace)
        {
            SetPosition(finalPosition);
        }
    }

    void SetPosition(Vector3 targetPosition)
    {
        Transform placementPrefabTransform = currentStructure.placementPrefab.transform;
        Vector3 positionVelocity = Vector3.zero;

        if(Vector3.Distance(placementPrefabTransform.position, targetPosition) > 10)
        {
            placementPrefabTransform.position = targetPosition;
        }
        else
        {
            Vector3 newTargetPosition = Vector3.SmoothDamp(placementPrefabTransform.position, targetPosition, ref positionVelocity, 0, 15000);
            placementPrefabTransform.position = newTargetPosition;
        }
    }

    void ChangeStructureType(Structure newStructure)
    {
        buildSystemUIPanel.gameObject.SetActive(true);

        systemEnabled = true;

        currentStructure = newStructure;

        foreach (var structure in structures)
        {
            structure.placementPrefab.SetActive(structure.structureType == currentStructure.structureType);
        }

        //Vider les couts a l'ecran
        foreach (Transform child in buildSystemUIPanel)
        {
            Destroy(child.gameObject);
        }

        //ajouter les couts de la structure a placer
        foreach (ItemInInventory requiredRessource in currentStructure.ressourcesCost)
        {
            GameObject requiredElementGO = Instantiate(buildingRequiredElement, buildSystemUIPanel);
            requiredElementGO.GetComponent<BuildingRequiredElement>().Setup(requiredRessource);
        }

        UpdateDisplayedCosts();
    }

    private Structure GetStructureByType(StructureType structureType)
    {
        return structures.Where(elem => elem.structureType == structureType).FirstOrDefault();    
    }

    public void UpdateDisplayedCosts()
    {
        //Vider les couts a l'ecran
        foreach (Transform child in buildSystemUIPanel)
        {
            Destroy(child.gameObject);
        }

        //ajouter les couts de la structure a placer
        foreach (ItemInInventory requiredRessource in currentStructure.ressourcesCost)
        {
            GameObject requiredElementGO = Instantiate(buildingRequiredElement, buildSystemUIPanel);
            requiredElementGO.GetComponent<BuildingRequiredElement>().Setup(requiredRessource);
        }
    }

    public void LoadStructures(PlacedStructure[] structuresToLoad)
    {
        foreach (PlacedStructure structure in structuresToLoad)
        {
            placedStructures.Add(structure);
            GameObject newStructure = Instantiate(structure.prefab, placeStructuresParent);
            newStructure.transform.position = structure.positions;
            newStructure.transform.rotation = Quaternion.Euler(structure.rotations);
        }
    }
}

[System.Serializable]
public class Structure
{
    public GameObject placementPrefab;
    public GameObject instantiatedPrefab;
    public StructureType structureType;
    public ItemInInventory[] ressourcesCost;
}

public enum StructureType
{
    Stairs,
    Wall,
    Floor
}

[System.Serializable]
public class PlacedStructure
{
    public GameObject prefab;
    public Vector3 positions;
    public Vector3 rotations;
}