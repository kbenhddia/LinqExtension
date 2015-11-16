LINQExtension
=============

LINQExtension is a free solution for Dynamic Linq to Entities &amp; querying by criteria in entity framework<br>
It may be helpful for who he has problems with linq as langage

<h3> Dynamic Linq to Entities & querying by criteria in entity framework </h3><br>
we are using this Employees table as an example<br>
<table>
<tr>
<th>Column</th><th>		Type</th>
</tr>
<tr>
<td>EmployeeID</td><td>	int</td>
</tr>
<tr>
<td>NationalIDNumber</td><td>	int</td>
</tr>
<tr>
<td>Title</td><td>	varchar(20)</td>
</tr>
<tr>
<td>HireDate</td><td>	datetime</td>
</tr>
<tr>
<td>BirthDate</td><td>	datetime</td>
</tr>
<tr>
<td>Salary</td><td>	float</td>
</tr>
<tr>
<td>VacationHours</td><td>	int</td>
</tr>
<tr>
<td>VacationHours</td><td>	int</td>
</tr>
<tr>
<td>EmployeeID</td><td>	datetime</td>
</tr>
</table>
<br>
<b> The Problem: </b><br>
We want to find data with LINQ TO ENTITIES by criteria; we need to check the value entered for each input before invoking the query.<br>
<ul>
<li>
If Title! = Null 
</li>
<li>
If EmployeeID> 0 
</li>
<li>
If NationalIDNumber.lenght  ==8
</li>
<li>
If HireDate <= DateTime.Now
</li>
</ul><br>
to obtain your final request; your code will contain multiple IF ... Else .However, It will be a hard to write and we got an ambiguous code.<br>

```csharp
if (!String.IsNullOrEmpty(input_Title))
{
	…
}
if (input_EmployeeID>0)
{
	…
}
if (input_NationalIDNumber>0)
{
	…
}
…
```

<b>Solution 1: Finding by criteria</b><br>

The solution with LINQExtension is simply to call WhereIf, this method is created to search with a number of criteria known in advance. <br>
```csharp
using (var context = new workEntities())
     {
                var input_EmployeeID = 5;
                var input_NationalIDNumber = 0587456;
                var input_TitleJob = "Developper";

                var data = context.Employee
            		.WhereIf(input_EmployeeID != 0, 
                         e => e.EmployeeID == input_EmployeeID)
     		        .WhereIf(input_NationalIDNumber != 0, 
                         e => e.NationalIDNumber == input_NationalIDNumber)
            		.WhereIf(!String.IsNullOrEmpty(input_Title),
                         e => e.Title == input_Title)
                .ToList();
}

```
<br>
<br><b>Solution 2 (Advanced Topic): Dynamic Linq</b><br>
We don’t know the number of parameters introduced in advance and we want to do a blind research. 
<ul>
<li> No criteria => select * from Emloyee  </li>
<li>One criteria ($ 1) => select * from Employee where col1 = $ 1  </li>
<li>Two criterias ($ 1, $ 2) => select * from Employee where col1 = $ 1 and col2 = $ 2 </li>
<li>Three criterias....</li>
</ul><br>

CollectionToQuery is created for this kind of query, it doesn’t depend on number of parameters, it uses ``` Dictionary <string, List <string>> ``` to collect the information's research.<br>
This method implements [LinqKit] library (http://www.albahari.com/nutshell/linqkit.aspx ).<br>
```csharp
using (var context = new workEntities() )
{

	Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
	dictionary["Title"] = new List<string> {  
					"Network Engineer", 
					"Security Specialist", 
					"=Web Developer"
	 			};
	dictionary["Salary"] = new List<string> { ">=2000" };
	dictionary["VacationHours"] = new List<string> { ">21" };
	dictionary["SickLeaveHours"] = new List<string> { "<5" };                
	dictionary["HireDate"] = new List<string> { 
					">=01/01/2000",
					"28/02/2014" 
				};
	dictionary["ModifiedDate"] = new List<string> { DateTime.Now.ToString() };
	
	var data = context.Employee.CollectionToQuery(dictionary).ToList();
}

```
<br>
<b>The query result:</b>
![](http://s8.postimg.org/piesznj1h/dunamiclinq.png)<br><br>

<b>You need to know</b><br>
This code doesn't support joins; you can use views in this case.<br>
<b>Licensing</b><br>
LINQExtension is free. The source code is issued under a permissive free license, which means you can modify it as you like, and incorporate it into your own commercial or non-commercial software. Enjoy!
