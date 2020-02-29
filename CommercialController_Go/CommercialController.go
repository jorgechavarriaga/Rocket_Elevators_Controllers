/**********************************************************************************************************
*** PROGRAM:      CODEBOXX ODSSEY                                                                       ***
*** WEEK 3:       THE MECHANICS OF COMPILDE LANGUAGES                                                   ***
*** DELIVERABLE:  RESIDENTIAL CONTROLLER - GO                                                           ***
*** BY:           JORGE CHAVARRIAGA                                                                     ***
*** DATE:         28-FEB-2020                                                                           ***
***********************************************************************************************************/

package main 

import ("fmt")

func main(){
 

    //way 1
    for i :=0; i < 5; i++{
        fmt.Println(i)
    }

    fmt.Println()
    
    //way 2
    i :=0
    for i<5{
        fmt.Println(i)
        i++
    }
    
    fmt.Println()
    
    for i:=0; i<10; i++{
        for j:=0; j<i; j++{
            fmt.Printf("*")
        }
        fmt.Println()
    }	


}  