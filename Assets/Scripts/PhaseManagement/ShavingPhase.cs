using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace GrommingMaster
{
    public class ShavingPhase : PhaseManagement
    {
        [Header("Individual Properties")]       
        [SerializeField] GameObject Shaver;
        [SerializeField] Transform Necklace;
        [SerializeField] MeshCollider DogHairLess;
        [SerializeField] MeshCollider ReflectPlane;
        [SerializeField] MeshCollider DogFur;

        [Header("UI Properties")]
        [SerializeField] Slider ProgressBar;
        [SerializeField] RectTransform TargetPanel;
        [SerializeField] RectTransform MovingPanel;        

        [Header("Special Properties")]
        [SerializeField] ShaveType shaveType;
        public enum ShaveType { Fur, Body };

        // Shaving Body Properties
        Mesh targetMesh;
        Ray ray;
        Vector3[] vertices;
        Vector3[] normals;
        bool[] isPushed;
        Dictionary<int, List<int>> neighborVertex = new Dictionary<int, List<int>>();

        // Turning Properties        
        Side turnDirection;        
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
                if (Shaver.GetComponent<Shaver>().CanUse)
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

                // rotate the camera & shaver
                if (MovingPanel != null)
                {
                    switch (turnDirection)
                    {
                        case Side.Left:
                            Camera.main.transform.RotateAround(DogTransform.position, Vector3.up, 50 * Time.deltaTime);
                            Shaver.transform.RotateAround(DogTransform.position, Vector3.up, 50 * Time.deltaTime);
                            break;
                        case Side.Right:
                            Camera.main.transform.RotateAround(DogTransform.position, Vector3.up, -50 * Time.deltaTime);
                            Shaver.transform.RotateAround(DogTransform.position, Vector3.up, -50 * Time.deltaTime);
                            break;
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (ProgressBar.value == ProgressBar.maxValue) EndPhase();
                turnDirection = Side.None;                
            }
        }

        void FurShaving(Ray ray)
        {
            RaycastHit[] hitObjects = Physics.SphereCastAll(ray, 0.3f, Mathf.Infinity);

            for (int i = 0; i < hitObjects.Length; i++)
            {
                if (hitObjects[i].transform.CompareTag("Can Shave"))
                {
                    Destroy(hitObjects[i].transform.gameObject);
                    Shaver.GetComponent<Shaver>().Fur.Play();
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
                    foreach (var vertex in neighborVertex[index])
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
                    // Check if that vertice isn't pushed then push it
                    if (!isPushed[verticesToPush[j]])
                    {                        
                        // Get the vertice coordinates from local mesh to world space
                        position = hitFur.transform.TransformPoint(vertices[verticesToPush[j]]);

                        // Get the normal vector from mesh collider
                        direction = -hitFur.transform.TransformDirection(normals[verticesToPush[j]]);

                        // Raycast from vertice point to find the place to put the vertice onto the dog body                                                
                        if (DogHairLess.Raycast(new Ray(position, direction), out hitHairLess, 0.15f))
                        {
                            // Get the hit point from world space to local fur mesh (because vertice using local mesh to move)
                            position = hitFur.transform.InverseTransformPoint(hitHairLess.point);

                            // Put our vertice at that point                                    
                            vertices[verticesToPush[j]] = position - normals[verticesToPush[j]] * 0.0007f;

                            isPushed[verticesToPush[j]] = true;
                            Shaver.GetComponent<Shaver>().Fur.Play();
                            ProgressBar.value += 3.5f;
                        }
                    }
                }
                targetMesh.vertices = vertices;
                targetMesh.RecalculateNormals();
                
            }
            if (ReflectPlane.Raycast(ray, out RaycastHit hitPlane, Mathf.Infinity))
            {
                BodyShaving(new Ray(hitPlane.point, -ray.direction));
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
            StartCoroutine(IE_InitiateNeighborVertex());
        }

        IEnumerator IE_StartPhase()
        {
            yield return new WaitWhile(() => this.GameStatus == Status.Standing);

            StartCoroutine(GlowingFur());
            ProgressBar.value = 0;

            Camera.main.transform.DOMove(CameraTransform.position, 1);
            Camera.main.transform.DORotate(CameraTransform.localEulerAngles, 1);

            Necklace.gameObject.SetActive(true);
            Necklace.DOLocalMove(new Vector3(0, 0, 0), 1);

            // Moving UI
            TargetPanel.DOAnchorPosY(-250, 1.5f);
            MovingPanel.DOAnchorPosY(150, 1.5f);

            ProgressBar.GetComponent<RectTransform>().DOAnchorPosY(-150, 1.5f);
            ProgressBar.maxValue = targetMesh.vertexCount;

            Shaver.transform.DOMoveX(0, 1).OnComplete(() => GameStatus = Status.Playing);
        }

        IEnumerator IE_InitiateNeighborVertex()
        {
            float time = Time.time;
            int count = 0;
            targetMesh = DogFur.GetComponent<MeshFilter>().mesh;
            isPushed = new bool[targetMesh.vertexCount];

            for (int i = 0; i < targetMesh.triangles.Length; i++)
            {
                count++;
                if (count >= 50)
                {
                    count = 0;
                    yield return null;
                }

                // Adding the two other vertex in a triangle to the vertex neighbor list
                switch (i % 3)
                {
                    case 0:
                        if (neighborVertex.ContainsKey(targetMesh.triangles[i]))
                        {
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 2]);
                        }
                        else
                        {
                            neighborVertex.Add(targetMesh.triangles[i], new List<int>());
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 2]);
                        }
                        break;
                    case 1:
                        if (neighborVertex.ContainsKey(targetMesh.triangles[i]))
                        {
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                        }
                        else
                        {
                            neighborVertex.Add(targetMesh.triangles[i], new List<int>());
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i + 1]);
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                        }
                        break;
                    case 2:
                        if (neighborVertex.ContainsKey(targetMesh.triangles[i]))
                        {
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 2]);
                        }
                        else
                        {
                            neighborVertex.Add(targetMesh.triangles[i], new List<int>());
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 1]);
                            neighborVertex[targetMesh.triangles[i]].Add(targetMesh.triangles[i - 2]);
                        }
                        break;
                }
            }            
        }
        IEnumerator IE_EndPhase()
        {            
            Shaver.SetActive(false);

            Necklace.DOLocalMove(new Vector3(1, 1, 0), 1f);
            Necklace.gameObject.SetActive(false);

            Camera.main.transform.DOMove(CameraTransform.position, 0.75f);
            Camera.main.transform.DOLocalRotate(CameraTransform.localEulerAngles, 0.75f);
            yield return new WaitForSeconds(0.5f);

            if (ProgressBar.value == ProgressBar.maxValue)
            {
                DogFur.gameObject.SetActive(false);
                this.GameStatus = Status.Win;
            }
            else
            {
                this.GameStatus = Status.Lose;
            }

            SparkleParticles.SetActive(true);
            yield return new WaitForSeconds(2);

            SparkleParticles.SetActive(false);            
            // Move to next phase
            CurrentPhase.SetActive(false);
            NextPhase.GetComponent<PhaseManagement>().GameStatus = Status.Playing;
        }
        IEnumerator GlowingFur()
        {
            DogFur.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/RimLightOpaque");
            DogFur.GetComponent<MeshRenderer>().material.SetColor("_RimColor", Color.cyan);

            float startTime = Time.time;

            yield return new WaitUntil(() => (Input.GetMouseButtonDown(0) || Time.time - startTime > 2));
            
            DogFur.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
        }
    }
}