using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    [SerializeField]
    private int noOfColumns;

    [SerializeField]
    private int noOfRows;

    [SerializeField]
    private mazeNode nodePrefab;

    private mazeNode [,] mazeGrid;


    IEnumerator Start(){
        mazeGrid = new mazeNode[noOfColumns, noOfRows];
        CreateGrid();
        yield return CarveMaze(null, GetStartNode());
        
    }

    private mazeNode GetStartNode(){

        int xC = Random.Range(0, noOfColumns);
        int yC = Random.Range(0, noOfRows);

        return mazeGrid[xC,yC];
    }

    private void CreateGrid()
    {

        for (int i=0; i<  noOfColumns; i++){
            for(int j=0; j < noOfRows; j++){
               
                mazeGrid[i,j] = Instantiate(nodePrefab, new Vector3(i,j,1), Quaternion.identity);
            }
        }
    }

    private IEnumerator CarveMaze(mazeNode previousNode, mazeNode currentNode){
        currentNode.Visit();
        ClearWall(previousNode, currentNode);

        yield return new WaitForSeconds(0.2f);
        mazeNode nextNode;
        
        do{
            nextNode = GetNextUnvisitedNode(currentNode);

            if(nextNode != null){
            yield return CarveMaze(currentNode, nextNode);
            }
        } while(nextNode !=null);
        
    }

    private mazeNode GetNextUnvisitedNode(mazeNode currentNode){
        var unvisitedNodes = GetUnvisitedNodes(currentNode);

        return unvisitedNodes.OrderBy(n => UnityEngine.Random.Range(1,10)).FirstOrDefault();

    }

    private IEnumerable<mazeNode> GetUnvisitedNodes (mazeNode currentNode){
        int x = (int)currentNode.transform.position.x;
        int y = (int)currentNode.transform.position.y;

        if(x+1 < noOfColumns){
            var rightNode = mazeGrid[x+1,y];

            if(rightNode.isVisited == false)
            yield return rightNode;
        }

        if(x-1 >= 0){
            var leftNode = mazeGrid[x-1,y];

            if(leftNode.isVisited == false)
            yield return leftNode;
        }

        if(y+1 < noOfRows){
            var topNode = mazeGrid[x,y+1];

            if(topNode.isVisited == false)
            yield return topNode;
        }

        if(y-1 >=0){
            var bottomNode = mazeGrid[x, y-1];

            if(bottomNode.isVisited == false)
            yield return bottomNode;
        }
    }

    private void ClearWall(mazeNode previousNode, mazeNode currentNode){
        if(previousNode == null){
            return;
        } else if(previousNode.transform.position.x < currentNode.transform.position.x){
            previousNode.ClearRightWall();
            currentNode.ClearLeftWall();
            return;
        } else if (previousNode.transform.position.x > currentNode.transform.position.x){
            previousNode.ClearLeftWall();
            currentNode.ClearRightWall();
            return;
        } else if (previousNode.transform.position.y < currentNode.transform.position.y){
            previousNode.ClearTopWall();
            currentNode.ClearBottomWall();
            return;
        } else if (previousNode.transform.position.y > currentNode.transform.position.y){
            previousNode.ClearBottomWall();
            currentNode.ClearTopWall();
            return;
        }
    }
}