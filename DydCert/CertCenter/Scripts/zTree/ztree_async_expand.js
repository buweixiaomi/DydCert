//针对zTree异步加载封装的方法
//陈珊珊 2013.4.16

var currentTreeNode;
//当前选中节点
var rootId = "0";
//根节点ID 默认为0

//*************************获取节点或节点集合***************************

//根据节点编号寻找节点(返回第一个)

function zTree_GetNodeById(treeNodeId, treeId) {
    var node = null;
    var nodes = $.fn.zTree.getZTreeObj(treeId).getNodesByParam("id", treeNodeId, null);
    if (nodes != null && nodes.length > 0)
        node = nodes[0];
    return node;
}

//根据节点编号寻找节点集合

function zTree_GetNodeListById(treeNodeId, treeId) {
    var nodes = $.fn.zTree.getZTreeObj(treeId).getNodesByParam("id", treeNodeId, null);
    return nodes;
}

//根据节点名称寻找节点集合

function zTree_GetNodeListByName(treeNodeName, treeId) {
    var nodes = $.fn.zTree.getZTreeObj(treeId).getNodesByParam("name", treeNodeName, null);
    return nodes;
}

//*************************层级展开，选中节点***************************

//从treeNode节点向上展开到根节点

function zTree_ExpandToRootNode(treeNode, treeId) {
    $.fn.zTree.getZTreeObj(treeId).expandNode(treeNode, true);
    if (treeNode.pId == rootId)
        $.fn.zTree.getZTreeObj(treeId).expandNode(treeNode.pId, true);
    else {
        var node = treeNode.getParentNode();
        if (node != null) {
            if (node.pId == rootId)
                $.fn.zTree.getZTreeObj(treeId).expandNode(node, true);
            else
                zTree_ExpandToRootNode(node, treeId);
        }
    }
}

//选中节点（滚动条定位）,并层级展开上级节点

function zTree_SelectNode(treeNode, treeId) {
    zTree_ExpandToRootNode(treeNode, treeId);

    $.fn.zTree.getZTreeObj(treeId).selectNode(treeNode);
    currentTreeNode = treeNode;
    zTree_LocateCurrentNode(treeId);
}

//根据节点编号选中节点,并层级展开上级节点

function zTree_SelectNodeById(treeNodeId, treeId) {
    var treeNode = zTree_GetNodeById(treeNodeId, treeId);
    if (treeNode == null) //未加载出,递归找出该节点
        zTree_asyncSelectNode(rootId, treeNodeId, treeId);
    else
        zTree_SelectNode(treeNode, treeId);
}

//加载未加载出的节点，知道出现treeNodeId

function zTree_asyncSelectNode(currentId, treeNodeId, treeId) {
    if (currentId != treeNodeId) {
        var nodes = zTree_GetNodeListById(currentId, treeId);
        if (nodes != null && nodes.length > 0) {
            for (var i = 0; i < nodes.length; i++) {
                $.fn.zTree.getZTreeObj(treeId).expandNode(nodes[i], true);
                if (nodes[i].id == treeNodeId) {
                    zTree_SelectNodePureById(treeNodeId, treeId);
                    break;
                } else {
                    var childnodes = nodes[i].children;
                    if (childnodes != null && childnodes.length > 0) {
                        for (var j = 0; j < nodes.length; j++)
                            zTree_asyncSelectNode(childnodes[j].id, treeNodeId, treeId);
                    }
                }
            }
        }
    }
}


//根据节点编号选中节点

function zTree_SelectNodePureById(treeNodeId, treeId) {
    var treeNode = zTree_GetNodeById(treeNodeId, treeId);
    if (treeNode != null) {
        $.fn.zTree.getZTreeObj(treeId).selectNode(treeNode);
        currentTreeNode = treeNode;
        zTree_LocateCurrentNode(treeId);
    }
}

//*************************对当前节点操作（包括新增、修改、删除、上移、下移）***************************
//刷新当前节点下的节点（用于新增、刷新等）（type 为 add 或 edit  默认add）

function zTree_refreshCurrentNode(treeId, type) {
    if (type != "edit") {
        if (!currentTreeNode.isParent)
            currentTreeNode.isParent = true;
    }
    if (currentTreeNode.id != rootId)
        $.fn.zTree.getZTreeObj(treeId).reAsyncChildNodes(currentTreeNode, "refresh");
    else
        $.fn.zTree.getZTreeObj(treeId).reAsyncChildNodes(null, "refresh");
}

//更新当前节点(json: node的json格式)

function zTree_UpdateCurrentNode(json, treeId) {
    currentTreeNode.name = json.name;
    currentTreeNode.iconSkin = json.iconSkin;
    $.fn.zTree.getZTreeObj(treeId).updateNode(currentTreeNode);
    zTree_SelectNode(currentTreeNode, treeId);
}

//删除当前节点

function zTree_RemoveCurrentNode(treeId) {
    $.fn.zTree.getZTreeObj(treeId).removeNode(currentTreeNode);
    currentTreeNode = null;
}

//当前节点上移

function zTree_UpCurrentNode(treeId) {
    $.fn.zTree.getZTreeObj(treeId).moveNode(currentTreeNode.getPreNode(), currentTreeNode, "prev");
    zTree_LocateCurrentNode(treeId);
}

//当前节点下移

function zTree_DownCurrentNode(treeId) {
    $.fn.zTree.getZTreeObj(treeId).moveNode(currentTreeNode.getNextNode(), currentTreeNode, "next");
    zTree_LocateCurrentNode(treeId);
}

//当前节点移动到编号为targetTreeNodeId节点下面

function zTree_MoveCurrentNode(targetTreeNodeId, treeId) {
    $.fn.zTree.getZTreeObj(treeId).removeNode(currentTreeNode);
    zTree_refreshNode(targetTreeNodeId, treeId);
}

//当前节点定位（控制滚动条）

function zTree_LocateCurrentNode(treeId) {
    if ($("#" + currentTreeNode.tId).offset().top + document.getElementById(treeId).scrollTop > $(window).height() - 30)
        document.getElementById(treeId).scrollTop = $("#" + currentTreeNode.tId).offset().top + document.getElementById(treeId).scrollTop - ($(window).height() - 30) + 20;
}

//*************************其他操作（包括修改、删除、上移、下移、批量删除）***************************
//在指定节点下新增一系列节点(json: nodes的json格式) (isSelect：是否选中节点)

function zTree_AddNodes(json, treeNodeId, treeId, isSelect) {
    if (treeNodeId != null) {
        var node = zTree_GetNodeById(treeNodeId, treeId);
        if (node != null) {
            var nodes = $.fn.zTree.getZTreeObj(treeId).addNodes(node, json);
            if (nodes.length > 0 && isSelect)
                zTree_SelectNode(nodes[0], treeId);
        }
    } else {
        var nodes = $.fn.zTree.getZTreeObj(treeId).addNodes(null, json);
        if (nodes.length > 0 && isSelect)
            zTree_SelectNode(nodes[0], treeId);
    }
}

//刷新指定节点下的节点（用于新增、刷新、复制等）

function zTree_refreshNode(treeNodeId, treeId) {
    var node = zTree_GetNodeById(treeNodeId, treeId);
    if (node != null) {
        if (!node.isParent)
            node.isParent = true;
        if (node.id != rootId)
            $.fn.zTree.getZTreeObj(treeId).reAsyncChildNodes(node, "refresh");
        else
            $.fn.zTree.getZTreeObj(treeId).reAsyncChildNodes(null, "refresh");
    }
}

//更新节点(json: node的json格式)

function zTree_UpdateNode(json, treeId, isSelect) {
    var node = zTree_GetNodeById(json.id, treeId);
    if (node != null) {
        node.name = json.name;
        node.iconSkin = json.iconSkin;
        $.fn.zTree.getZTreeObj(treeId).updateNode(node);
        if (isSelect)
            zTree_SelectNodeById(node.id, treeId);
    }
}

//删除节点(适用于已经加载出的节点,否则建议调用zTree_refreshNode方法，刷新父节点)

function zTree_RemoveNode(treeNodeId, treeId) {
    var node = zTree_GetNodeById(treeNodeId, treeId);
    if (node != null)
        $.fn.zTree.getZTreeObj(treeId).removeNode(node);
}

//节点上移(适用于已经加载出的节点,否则建议调用zTree_refreshNode方法，刷新父节点)

function zTree_UpNode(treeNodeId, treeId, isSelect) {
    var node = zTree_GetNodeById(treeNodeId, treeId);
    if (node != null) {
        $.fn.zTree.getZTreeObj(treeId).moveNode(node.getPreNode(), node, "prev");
        if (isSelect)
            zTree_SelectNodeById(node.id, treeId);
    }
}

//节点下移(适用于已经加载出的节点,否则建议调用zTree_refreshNode方法，刷新父节点)

function zTree_DownNode(treeNodeId, treeId, isSelect) {
    var node = zTree_GetNodeById(treeNodeId, treeId);
    if (node != null) {
        $.fn.zTree.getZTreeObj(treeId).moveNode(node.getNextNode(), node, "next");
        if (isSelect)
            zTree_SelectNodeById(node.id, treeId);
    }
}

//批量在树上删除节点(适用于已经加载出的节点,否则建议调用zTree_refreshNode方法，刷新父节点)

function zTree_RemoveNodes(nodeIdList, treeId) {
    for (var i = 0; i < nodeIdList.length; i++) {
        if (nodeIdList[i] != "") {
            var nodes = zTree_GetNodeListById(nodeIdList[i], treeId);
            if (nodes != null && nodes.length > 0)
                $.fn.zTree.getZTreeObj(treeId).removeNode(nodes[0]);
        }
    }
}

//节点定位（控制滚动条）

function zTree_LocateNode(treeNode, treeId) {
    if ($("#" + treeNode.tId).offset().top + document.getElementById(treeId).scrollTop > $(window).height() - 30)
        document.getElementById(treeId).scrollTop = $("#" + treeNode.tId).offset().top + document.getElementById(treeId).scrollTop - ($(window).height() - 30) + 20;
}


//编辑当前节点下的所有子节点

function zTree_transformToArray(treeId, treeNode) {
    if (treeNode != null) {
        return $.fn.zTree.getZTreeObj(treeId).transformToArray(treeNode);
    }
    return $.fn.zTree.getZTreeObj(treeId).transformToArray(treeNode);
}

//获取节点层级

function zTree_getNodeIndex(treeId, treeNode) {
    if (treeNode != null) {
        return $.fn.zTree.getZTreeObj(treeId).getNodeIndex(treeNode);
    }
    return $.fn.zTree.getZTreeObj(treeId).getNodeIndex(treeNode);
}
