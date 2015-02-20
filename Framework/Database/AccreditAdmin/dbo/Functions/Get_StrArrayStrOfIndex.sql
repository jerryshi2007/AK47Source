
CREATE function [dbo].[Get_StrArrayStrOfIndex]  
(  
	@str varchar(1024), --要分割的字符串  
	@split varchar(10), --分隔符号  
	@index int --取第几个元素  
)  
returns varchar(1024)  
as  
begin  
declare @location int  
declare @start int  
declare @next int  
declare @seed int  

	set @str=ltrim(rtrim(@str))  
	set @start=1  
	set @next=1  
	set @seed=len(@split)  

	set @location=charindex(@split,@str)  
	while @location<>0 and @index>@next  
	begin  
		set @start=@location+@seed  
		set @location=charindex(@split,@str,@start)  
		set @next=@next+1  
	end  
	if @location =0 select @location =len(@str)+1 
	 
	 return substring(@str,@start,@location-@start)  
end  

