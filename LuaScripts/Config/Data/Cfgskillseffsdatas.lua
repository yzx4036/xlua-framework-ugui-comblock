local Cfgskillseffsdatas ={
}

function Cfgskillseffsdatas.GetByKey(key)
	if Cfgskillseffsdatas[key] == nil then
		LogError('Cfgskillseffsdatas 配置没有key对应:',key)
	end
	return Cfgskillseffsdatas[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite

return Cfgskillseffsdatas