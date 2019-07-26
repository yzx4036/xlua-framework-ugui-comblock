---
--- Created by Administrator.
--- DateTime: 2017/12/14 11:06
---

local print = print
local tconcat = table.concat
local tinsert = table.insert
local type = type
local pairs = pairs
local tostring = tostring
local next = next

local function pr (t, name, indent)
	local tableList = {}
	function table_r (t, name, indent, full)
		local id = not full and name or type(name)~="number" and tostring(name) or '['..name..']'
		local tag = indent .. id .. ' = '
		local out = {}  -- result
		if type(t) == "table" then
			if tableList[t] ~= nil then
				tinsert(out, tag .. '{} -- ' .. tableList[t] .. ' (self reference)')
			else
				tableList[t]= full and (full .. '.' .. id) or id
				if next(t) then -- Table not empty
					tinsert(out, tag .. '{')
					for key,value in pairs(t) do
						tinsert(out,table_r(value,key,indent .. '|  ',tableList[t]))
					end
					tinsert(out,indent .. '}')
				else tinsert(out,tag .. '{}') end
			end
		else
			local val = type(t)~="number" and type(t)~="boolean" and '"'..tostring(t)..'"' or tostring(t)
			tinsert(out, tag .. val)
		end
		return tconcat(out, '\n')
	end
	return table_r(t,name or 'Value',indent or '')
end
local function print_r (t, name)
	print(debug.traceback(pr(t,name), 2))
end

return print_r