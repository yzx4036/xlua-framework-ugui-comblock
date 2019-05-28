---@class CfgdialogsdatasTable
local CfgdialogsdatasTable ={
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

---@class Cfgdialogsdatas
local Cfgdialogsdatas = BaseClass('Cfgdialogsdatas')
function Cfgdialogsdatas:__init(data)
	self.id = data.id 
	self.headID = data.headID 
	self.isPlayerSay = data.isPlayerSay 
	self.sayname = data.sayname 
	self.title = data.title 
	self.body = data.body 
	self.menu1 = data.menu1 
	self.menu2 = data.menu2 
	self.menu3 = data.menu3 
	self.menu4 = data.menu4 
	self.menu5 = data.menu5 
	self.funcFailMsg = data.funcFailMsg 
	self.func1 = data.func1 
	self.funcargs1 = data.funcargs1 
	self.func2 = data.func2 
	self.funcargs2 = data.funcargs2 
	self.func3 = data.func3 
	self.funcargs3 = data.funcargs3 
	self.func4 = data.func4 
	self.funcargs4 = data.funcargs4 
	self.func5 = data.func5 
	self.funcargs5 = data.funcargs5 
	data = nil
end

----not overwrite>> the class custom ----

--可在这里写一些自定义函数

----<<not overwrite----


--->>>>--->>>>--->>>>--------我是分割线--------<<<<---<<<<---<<<<---



---@type table<number, Cfgdialogsdatas>
local _cfgInstDict = {}


---@class CfgdialogsdatasHelper
local CfgdialogsdatasHelper = BaseClass('CfgdialogsdatasHelper')

function CfgdialogsdatasHelper:InitAll()
	if table.count(CfgdialogsdatasTable) > 0 then
		for k, v in pairs(CfgdialogsdatasTable) do
			_cfgInstDict[k] = Cfgdialogsdatas.New(v)
			CfgdialogsdatasTable[k] = nil
		end
	end
end

---@return table<number, Cfgdialogsdatas>
function CfgdialogsdatasHelper:GetTable()
	self:InitAll()
	return _cfgInstDict
end

---@return Cfgdialogsdatas
function CfgdialogsdatasHelper:GetByKey(key)
	if CfgdialogsdatasTable[key] == nil  and _cfgInstDict[key] == nil  then
		Logger.LogError('CfgdialogsdatasTable 配置没有key=%s对应的行!',key) return
	end
	if _cfgInstDict[key] == nil  then
		_cfgInstDict[key] = Cfgdialogsdatas.New(CfgdialogsdatasTable[key])
		CfgdialogsdatasTable[key] = nil
	end
	return _cfgInstDict[key]
end

----not overwrite>> the helper custom ----

--可在这里写一些自定义函数

----<<not overwrite----
return CfgdialogsdatasHelper