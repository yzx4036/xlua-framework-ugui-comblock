---@class Cfgdialogs_datasTable
local Cfgdialogs_datasTable ={
	[10001001] = {id = 10001001,headID = 30011001,isPlayerSay = true,sayname = '11',title = '',body = '最近[#scff00#]村外树林[#sc#]出现很多妖魔，你去杀掉[#scff0000#]10个[#sc#]再回来找我，我会给你奖励的。[#nl#]你一直往东走可以看到一个旋转的光圈，那里便是出村的路口了。',menu1 = 10001002,menu2 = 10001003,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = '',funcargs1 = '',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[10001002] = {id = 10001002,headID = 0,isPlayerSay = false,sayname = '',title = '不就是几个小妖物，何惧之有。',body = '这些村民真是没用，几个小妖就一副如临大敌的样子，我倒要去瞧瞧究竟是何妖物。',menu1 = 10001004,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = '',funcargs1 = '',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[10001003] = {id = 10001003,headID = 0,isPlayerSay = false,sayname = '',title = '我没兴趣。',body = '',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'closedialog',funcargs1 = '',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[10001004] = {id = 10001004,headID = 0,isPlayerSay = false,sayname = '',title = '现在出发',body = '',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'closedialog',funcargs1 = '',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[20001001] = {id = 20001001,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10011002,25`216,0',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[20001002] = {id = 20001002,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10011001,545`216,180',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[20001003] = {id = 20001003,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10012002,150`216,0',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[20001004] = {id = 20001004,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10012003,180`216,0',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[30001001] = {id = 30001001,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '爱护地球爱护植物',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10013001,210`120,0',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[30001002] = {id = 30001002,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '叽叽叽叽……',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10013002,210`120,0',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[30001003] = {id = 30001003,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '是人！是人啊，可怕的人来了',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10013003,210`120,0',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''},
	[30001004] = {id = 30001004,headID = 0,isPlayerSay = false,sayname = '',title = '',body = '你擅闯竹林，准备受死吧！人类',menu1 = 0,menu2 = 0,menu3 = 0,menu4 = 0,menu5 = 0,funcFailMsg = '',func1 = 'teleport',funcargs1 = '10013004,210`120,0',func2 = '',funcargs2 = '',func3 = '',funcargs3 = '',func4 = '',funcargs4 = '',func5 = '',funcargs5 = ''}
}

---@class Cfgdialogs_datas
local Cfgdialogs_datas = BaseClass('Cfgdialogs_datas')
function Cfgdialogs_datas:__init(data)
	self.id = data[1] 
	self.headID = data[2] 
	self.isPlayerSay = data[3] 
	self.sayname = data[4] 
	self.title = data[5] 
	self.body = data[6] 
	self.menu1 = data[7] 
	self.menu2 = data[8] 
	self.menu3 = data[9] 
	self.menu4 = data[10] 
	self.menu5 = data[11] 
	self.funcFailMsg = data[12] 
	self.func1 = data[13] 
	self.funcargs1 = data[14] 
	self.func2 = data[15] 
	self.funcargs2 = data[16] 
	self.func3 = data[17] 
	self.funcargs3 = data[18] 
	self.func4 = data[19] 
	self.funcargs4 = data[20] 
	self.func5 = data[21] 
	self.funcargs5 = data[22] 
	data = nil
end


---@type table<number, Cfgdialogs_datas>
local _instList={}

function Cfgdialogs_datasTable.InitAll()
	if table.length(Cfgdialogs_datasTable) > 0 then
		for k, v in pairs(Cfgdialogs_datasTable) do
			_instList[k] = Cfgdialogs_datas.New(v)
			Cfgdialogs_datasTable[k] = nil
		end
	end
end

---@return table<number, Cfgdialogs_datas>
function Cfgdialogs_datasTable.GetTable()
	Cfgdialogs_datasTable.InitAll()
	return _instList;
end

---@return Cfgdialogs_datas
function Cfgdialogs_datasTable.GetByKey(key)
	if Cfgdialogs_datasTable[key] == nil  and _instList[key] == nil  then
		Logger.LogError('Cfgdialogs_datasTable 配置没有key=%s对应的行!',key) return
	end
	if _instList[key] == nil  then
		_instList[key] = Cfgdialogs_datas.New(Cfgdialogs_datasTable[key])
		Cfgdialogs_datasTable[key] = nil
	end
	return _instList[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite
return Cfgdialogs_datasTable