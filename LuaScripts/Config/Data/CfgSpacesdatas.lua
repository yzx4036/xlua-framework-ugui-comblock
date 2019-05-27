local CfgSpacesdatas ={
	[1] = {entityType = 'Space',name = '新手村',id = 1,type = 1,spawnPos = {771.5861,211.0021,776.5501},resPath = 'spaces/xinshoucun'},
	[2] = {entityType = 'Space',name = 'kbengine_ogre_demo',id = 2,type = 1,spawnPos = {-97.9299,0.0,-158.922},resPath = 'spaces/kbengine_ogre_demo'}
}

function CfgSpacesdatas.GetByKey(key)
	if CfgSpacesdatas[key] == nil then
		LogError('CfgSpacesdatas 配置没有key对应:',key)
	end
	return CfgSpacesdatas[key]
end

----not overwrite----

--可在这里写一些自定义函数

--not overwrite

return CfgSpacesdatas