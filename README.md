声明：本仓库非原项目，原项目请移步 https://github.com/smilehao/xlua-framework 原作者：smilehao
-

# xlua-framework
Unity游戏纯lua客户端完整框架---基于xlua，后续整合Kbengine服务端

2019.6.25 
-
1、添加sqlite数据库支持，不需要的可自行删除SQLite4Unity3d文件夹

2、assetbundle加载的www改为UnityWebRequest 

3、添加LuaUIGeneratorTool 功能，支持选中UIPrefab生成适配框架的UI模块MVC代码， 支持生成UIXXView.lua、UIXXCtrl.lua、UIXXModel，生成对应配置 注意：生成文件位置为选中目录的Temp目录下，根据自身需求，移动到真正目录，并需要手动在UIConfig.lua的UIModule添加新生成的模块，UIWindowNames.lua同理，后续添加生成组件到lua文件d功能

4、修复ios assetbundle加载为空的问题

2019.5.28 
-

重写配置表模块（生成工具，编辑器扩展，生成逻辑）

1、支持自动忽略覆盖自定义方法

2、支持导出配置表获取接口到ConfigCfgManager

3、配置表编辑器扩展重写


---未完成，持续更新中...

-------------------------------------------------------------------

XLua框架设计文档目录如下（具体内容查看工程下的《XLua框架设计文档.docx》）
-
一．总体设计	4
-
1.1 概述	4

1.2 工程目录	4

1.3 游戏启动流程	9

1.4 运行指导	9

二．Lua简介	15
-

2.1 lua设计语言简介	15

2.2 XLua简介	15

2.3 Unity侧功能的Lua实现	18

2.3 Lua通用扩展工具类	20

2.4 Lua面向对象程序设计	21

2.5 Lua数据表和常量表	25

2.6 Lua单例类	26

三．UI管理模块	28
-

3.1 脚本目录结构	28

3.2 UI模块添加流程	28

3.3 UI框架总体设计	28

3.4 组件系统	30

3.5层级管理	33

3.6 窗口记录栈	34

四．协程管理	36
-

4.1概述	36

4.2 协程操作	36

4.3 技术要点	39

4.4 其它说明	40

五．定时器管理	41
-

5.1 概述	41

5.2 驱动原理	41

六．资源管理模块	42
-

6.1 概述	42

6.2 AssetBundle设计概要	42

6.3 AssetBundle加载机制	43

6.4 AssetBundle打包机制	45

6.5 AssetBundle编辑器工具	47

6.6 热更新流程	60

6.7 资源预加载	72

6.8 资源缓存池	72

七．场景管理模块	74
-

7.1 概述	74

7.2 工作流程	74

7.3 技术要点	74

八．网络管理模块	77
-
删除原有的protobuf
todo 使用kbengine作为服务端

九．配置表	80
-

9.1 概述	80

9.2 xlsx gen lua	80

9.3 proto gen lua	80

十．XLua工作流	81
-

10.1 Lua脚本分类	81

10.2 XLua热修复	82

10.3 XLua动态库构建	83

10.4 XLua第三方库集成	87

10.5 XLua升级	88

十一．其它说明	90
-

11.1 资料和链接	90

11.2 Git地址	90

11.3 其它	91


Acknowledgements
-

This project is based on the work of:

    xlua-framework - https://github.com/smilehao/xlua-framework 
    
    Sqlite-net - License: custom - see https://github.com/praeclarum/sqlite-net/blob/master/LICENSE
    
    SQLite4Unity3d - https://github.com/robertohuertasm/SQLite4Unity3d/blob/master/LICENSE
    


 
