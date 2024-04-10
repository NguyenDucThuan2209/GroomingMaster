using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace GrommingMaster
{
    public class CuttingHairPhase : PhaseManagement
    {
        [Header("Individual Properties")]
        [SerializeField] GameObject Scissor;  
        
        [Header("Body Shave's Type Properties")]
        [SerializeField] MeshCollider DogHairLess;        
        [SerializeField] MeshCollider DogFur;
        [SerializeField] Transform Necklace;

        [Header("UI Properties")]
        [SerializeField] Slider ProgressBar;
        [SerializeField] RectTransform TargetPanel;
        [SerializeField] RectTransform NextPhaseButton;

        [Header("Special Properties")]
        [SerializeField] ShaveType shaveType;
        public enum ShaveType { Fur, Body };

        // Shaving Properties
        Mesh targetMesh;
        Ray ray;
        Vector3[] vertices;
        Vector3[] normals;
        Dictionary<int, List<int>> neighborVertices = new Dictionary<int, List<int>>();
        bool[] isPushed;

        // Turning Properties        
        Side turnDirection;
        float countTime = 0, requiredTime = 0.1f;
        public enum Side { None, Left, Right }

        // Start is called before the first frame update
        void Start()
        {
            StartPhase();
            if (shaveType == ShaveType.Body) InitiateNeighborVertex();
        }

        // Update is called once per frame
        void Update()
        {
            switch (GameStatus)
            {
                case Status.Win:
                case Status.Lose:
                case Status.Standing:
                    return;
            }

            if (Input.GetMouseButton(0))
            {
                // Shaving the dog's fur
                if (Scissor.GetComponent<Scissor>().CanUse)
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    switch (shaveType)
                    {
                        case ShaveType.Body:
                            BodyShaving(ray);
                            break;
                        case ShaveType.Fur:
                            FurShaving(ray);
                            break;
                    }
                }

                // rotate the camera & Scissor
                if (countTime < requiredTime) countTime += Time.deltaTime;
                else
                {
                    switch (turnDirection)
                    {
                        case Side.Left:
                            Camera.main.transform.RotateAround(DogTransform.position, Vector3.up, 50 * Time.deltaTime);
                            Scissor.transform.RotateAround(DogTransform.position, Vector3.up, 50 * Time.deltaTime);
                            break;
                        case Side.Right:
                            Camera.main.transform.RotateAround(DogTransform.position, Vector3.up, -50 * Time.deltaTime);
                            Scissor.transform.RotateAround(DogTransform.position, Vector3.up, -50 * Time.deltaTime);
                            break;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (ProgressBar.value + 20 >= ProgressBar.maxValue)
                {
                    this.GameStatus = Status.Win;
                    SparkleParticles.SetActive(true);
                    EndPhase();
                }
                turnDirection = Side.None;
                countTime = 0;
            }            
        }

        void FurShaving(Ray ray)
        {
            RaycastHit[] hitObjects = Physics.SphereCastAll(ray, 0.025f, Mathf.Infinity);

            for (int i = 0; i < hitObjects.Length; i++)
            {                
                if (hitObjects[i].transform.CompareTag("Can Shave"))
                {
                    ProgressBar.value++;
                    Destroy(hitObjects[i].transform.gameObject);
                    Scissor.GetComponent<Scissor>().Fur.Play();
                }
            }
        }
        void BodyShaving(Ray ray)
        {
            if (DogFur.Raycast(ray, out RaycastHit hitFur, Mathf.Infinity))
            {
                // Assign variables for exchange                                           
                RaycastHit hitHairLess; Vector3 position, direction;

                // Get the vertex that hit by raycast and all of its neighbor
                List<int> verticesToPush = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    int index = targetMesh.triangles[hitFur.triangleIndex * 3 + i];
                    foreach (var vertex in neighborVertices[index])
                    {
                        verticesToPush.Add(vertex);
                    }
                    verticesToPush.Add(index);
                }

                // Assgin vertices and normals of the target mesh
                vertices = targetMesh.vertices;
                normals = DogFur.sharedMesh.normals;

                // Loop through vertices from the triangle we hit in raycast progression
                for (int j = 0; j < verticesToPush.Count; j++)
                {
                    // Check if that vertice isn't pushed
                    if (!isPushed[verticesToPush[j]])
                    {
                        // Get the vertice coordinates from local mesh to world space
                        position = hitFur.transform.TransformPoint(vertices[verticesToPush[j]]);

                        // Get the normal vector from mesh collider
                        direction = -hitFur.transform.TransformDirection(normals[verticesToPush[j]]);

                        // Raycast from vertice point to find the place to put the vertice onto the dog body                                                
                        if (DogHairLess.Raycast(new Ray(position, direction), out hitHairLess, 0.05f))
                        {
                            Debug.DrawRay(position, direction, Color.cyan, 5);

                            // Get the hit point from world space to local fur mesh (because vertice using local mesh to move)
                            position = hitFur.transform.InverseTransformPoint(hitHairLess.point);

                            // Put our vertice at that point                                    
                            vertices[verticesToPush[j]] = position - normals[verticesToPush[j]] * 0.0007f;

                            isPushed[verticesToPush[j]] = true;
                            Scissor.GetComponent<Scissor>().Fur.Play();
                            ProgressBar.value += 10f;
                        }
                    }
                }
                targetMesh.vertices = vertices;
                targetMesh.RecalculateNormals();

            }            
        }
        void StartPhase()
        {
            StartCoroutine(IE_StartPhase());
        }
        public void EndPhase()
        {
            StartCoroutine(IE_EndPhase());
        }
        public void TurnCamera(bool type)
        {
            // true is right, false is left
            if (type)
            {
                turnDirection = Side.Right;
            }
            else
            {
                turnDirection = Side.Left;
            }

        }
        public void InitiateNeighborVertex()
        {
            float time = Time.time;
            targetMesh = DogFur.GetComponent<MeshFilter>().mesh;
            isPushed = new bool[targetMesh.vertexCount];

            for (int i = 0; i < targetMesh.triangles.Length; i++)
            {
                // Adding the two other vertex in a triangle to the vertex neighbor list
                switch (i % 3)
                {
                    case 0:
                        if (neighborVertices.ContainsKey(targetMesh.triangles[i]))
                        {
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 2]);
                        }
                        else
                        {
                            neighborVertices.Add(targetMesh.triangles[i], new List<int>());
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 2]);
                        }
                        break;
                    case 1:
                        if (neighborVertices.ContainsKey(targetMesh.triangles[i]))
                        {
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                        }
                        else
                        {
                            neighborVertices.Add(targetMesh.triangles[i], new List<int>());
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                        }
                        break;
                    case 2:
                        if (neighborVertices.ContainsKey(targetMesh.triangles[i]))
                        {
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 2]);
                        }
                        else
                        {
                            neighborVertices.Add(targetMesh.triangles[i], new List<int>());
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                            neighborVertices[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 2]);
                        }
                        break;
                }
            }
            Debug.Log("Normal time: " + (Time.time - time));
        }

        IEnumerator IE_StartPhase()
        {
            yield return new WaitWhile(() => this.GameStatus == Status.Standing);

            // Moving Camera
            Camera.main.transform.DOMove(CameraTransform.position, 1);
            Camera.main.transform.DORotate(CameraTransform.localEulerAngles, 1);

            // Moving UI
            TargetPanel.DOAnchorPosY(-250, 1.5f);
            NextPhaseButton.DOAnchorPosY(250, 1.5f);            
            ProgressBar.GetComponent<RectTransform>().DOAnchorPosY(-150, 1.5f);
            ProgressBar.value = 0;

            // Prepare for Phase
            StartCoroutine(GlowingFur());
            Scissor.SetActive(true);

            switch (shaveType)
            {
                case ShaveType.Body:                                                      
                    targetMesh = DogFur.GetComponent<MeshFilter>().mesh;
                    ProgressBar.maxValue = targetMesh.vertexCount;
                    isPushed = new bool[targetMesh.vertexCount];
                    
                    Necklace.DOLocalMove(new Vector3(1, 1, 0), 1f);
                    Necklace.gameObject.SetActive(false);                    
                    break;
                case ShaveType.Fur:

                    break;
            }
            
        }
        IEnumerator IE_EndPhase()
        {
            switch (shaveType)
            {
                case ShaveType.Body:
                    DogFur.gameObject.SetActive(false);
                    break;
                case ShaveType.Fur:
                    if (GameStatus == Status.Win)
                    {
                        for (int i = 0; i < DogTransform.childCount; i++)
                        {
                            if (DogTransform.GetChild(i).CompareTag("Can Shave"))
                            {
                                DogTransform.GetChild(i).gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < DogTransform.childCount; i++)
                        {
                            if (DogTransform.GetChild(i).CompareTag("Can Shave"))
                            {                                
                                DogTransform.GetChild(i).gameObject.AddComponent<PaintIn3D.P3dPaintable>();
                                DogTransform.GetChild(i).gameObject.AddComponent<PaintIn3D.P3dPaintableTexture>();
                                DogTransform.GetChild(i).gameObject.AddComponent<PaintIn3D.P3dMaterialCloner>();
                            }
                        }
                    }
                    break;
            }            
            Scissor.SetActive(false);           
            
            yield return new WaitForSeconds(2);

            SparkleParticles.SetActive(false);
            // Move to next phase
            CurrentPhase.SetActive(false);
            NextPhase.SetActive(true);
            NextPhase.GetComponent<PhaseManagement>().GameStatus = Status.Playing;
        }
        IEnumerator GlowingFur()
        {
            float startTime;
            switch (shaveType)
            {
                case ShaveType.Body:
                    DogFur.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/RimLightOpaque");
                    DogFur.GetComponent<MeshRenderer>().material.SetColor("_RimColor", Color.cyan);

                    startTime = Time.time;
                    yield return new WaitUntil(() => (Input.GetMouseButtonDown(0) || Time.time - startTime > 2));

                    DogFur.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
                    break;
                case ShaveType.Fur:
                    int count = 0;
                    for (int i = 0; i < DogTransform.childCount; i++)
                    {
                        if (DogTransform.GetChild(i).CompareTag("Can Shave"))
                        {
                            count++;
                            DogTransform.GetChild(i).GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/RimLight");
                            DogTransform.GetChild(i).GetComponent<MeshRenderer>().material.SetColor("_RimColor", Color.cyan);
                        }                                                
                    }
                    ProgressBar.maxValue = count;

                    startTime = Time.time;
                    yield return new WaitUntil(() => (Input.GetMouseButtonDown(0) || Time.time - startTime > 2));

                    for (int i = 0; i < DogTransform.childCount; i++)
                    {
                        if (DogTransform.GetChild(i).CompareTag("Can Shave"))
                        {
                            DogTransform.GetChild(i).GetComponent<MeshRenderer>().material.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
                        }
                    }
                    break;
            }            
        }
    }
}